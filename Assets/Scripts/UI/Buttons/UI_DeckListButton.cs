using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_DeckListButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Individual Component Related")]
    [SerializeField] private TextMeshProUGUI cardName = null;
    [SerializeField] private Image borderHighlight = null;
    [SerializeField] private Button myBtnComp = null;

    private Card myData = null;
    public Card CardData { get { return myData; } }

    private UI_DeckListManager deckListManager = null;
    private Coroutine deckRemoval;
    private int duplicateNumber = 1;
    private string cacheBaseName;
    private bool ranDecrement = false;

    public void InitializeButton(Card card, UI_DeckListManager manager)
    {
        if(cardName == null || borderHighlight == null)
        {
            Debug.LogWarning("Your button does not have the correct component on it.");
            return;
        }

        myData = card;

        cardName.text = myData.id;
        cacheBaseName = cardName.text;

        deckListManager = manager;

        if (myData.cardEffects.Contains(EnumCardEffects.Hero))
        {
            borderHighlight.color = Color.yellow;
        }
        else
        {
            borderHighlight.color = new Color(0f,0f,0f,0f);
        }
    }

    public void IncrementCardNumber()
    {
        duplicateNumber++;
        cardName.text = cacheBaseName + " x" + duplicateNumber;
    }

    public void DecrementCardNumber()
    {
        duplicateNumber--;
        if (duplicateNumber == 1) cardName.text = cacheBaseName;
        else if (duplicateNumber == 0) return;
        else cardName.text = cacheBaseName + " x" + duplicateNumber;
    }

    private void OnEnable()
    {
        if(myBtnComp == null)
        {
            Debug.LogWarning("This should have a button component.");
            return;
        }

        myBtnComp.onClick.AddListener(RemoveCardFromDeck);
    }

    private void OnDisable()
    {
        if (deckRemoval != null) StopCoroutine(deckRemoval);
        myBtnComp.onClick.RemoveListener(RemoveCardFromDeck);
    }

    private void RemoveCardFromDeck()
    {
        if(deckRemoval != null) return;
        deckRemoval = StartCoroutine(StartRemovingProcess());
    }

    private IEnumerator StartRemovingProcess()
    {
        if (myData == null)
        {
            Debug.LogWarning("Find out why this button doesn't have the reference to its own card.");
            yield return null;
        }

        var _cardRemoved = MainMenu_DeckManager.RemoveCardFromCurrentDeck(myData);
        if (!_cardRemoved)
        {
            Debug.LogWarning("Find out why the card data couldn't be removed from the card manager");
            yield return null;
        }

        if (!ranDecrement)
        {
            DecrementCardNumber();
            ranDecrement = true;
        }
 
        if (deckListManager == null)
        {
            Debug.LogWarning("Find out why you don't have a reference to the manager");
            yield return null;
        }
        StartCoroutine(deckListManager.RemoveFromCurrentButtons(myData, 
            (result) => {
                ranDecrement = false;
                if (duplicateNumber == 0) Destroy(gameObject);
            }));

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(myData == null)
        {
            Debug.LogWarning("Find out why this button doesn't have the reference to its own card.");
            return;
        }
        //Debug.LogWarning(myData.id + " going in card.");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (myData == null)
        {
            Debug.LogWarning("Find out why this button doesn't have the reference to its own card.");
            return;
        }
        //Debug.LogWarning(myData.id + " leaving card.");
    }
}
