using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_GraveyardCards : UI_CardViewScroll
{
    [Header("Graveyard Related")]
    [SerializeField] private Button m_closeButton = null;
    [SerializeField] private Button m_interactButton = null;
    [SerializeField] private GameObject m_objectToClose = null;
    [SerializeField] private AgileSelector m_agileSelector = null;

    private EnumUnitPlacement _cardPlace;

    protected override void Start()
    {
        base.Start();
        if (m_closeButton == null || m_objectToClose == null || m_interactButton == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "No reference for graveyard scene.");
            return;
        }

        if (m_agileSelector == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You can't choose agile cards.");
            return;
        }

        m_closeButton.gameObject.SetActive(true);
        m_interactButton.gameObject.SetActive(false);
        m_agileSelector.HideScreen();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        m_closeButton.onClick.AddListener(CloseGraveyard);
        m_interactButton.onClick.AddListener(InteractWithSelectedGraveyard);

        m_agileSelector.OnSelectedAgile += ShowInteractButton;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        m_closeButton.onClick.RemoveListener(CloseGraveyard);
        m_interactButton.onClick.RemoveListener(InteractWithSelectedGraveyard);

        m_agileSelector.OnSelectedAgile -= ShowInteractButton;
    }

    private void CloseGraveyard()
    {
        m_objectToClose.SetActive(false);
    }

    private void InteractWithSelectedGraveyard()
    {
        Debug.LogWarning("Trying to interact with graveyard.");
        _gameManager.SelectedGraveyardCardServerRpc(_cardToSelect, _cardSlot, _cardPlace);
    }

    public void AddCardsToGraveyard(List<Card> cardInfo)
    {
        if(m_viewTransform.childCount > 0)
        {
            int childCount = m_viewTransform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                GameObject child = m_viewTransform.GetChild(i).gameObject;
                Destroy(child);
            }

            _cardAnims.Clear();
            _buttons.Clear();
        }

        for (int i = 0; i < cardInfo.Count; i++)
        {
            var newCard = Instantiate(m_cardPrefab, m_viewTransform);
            var animComp = newCard.GetComponentInChildren<Anim_TransformUI>();
            var buttonComp = newCard.GetComponentInChildren<UI_ScrollCardButton>();

            if (animComp == null || buttonComp == null)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Missing components on Graveyard card.");
                return;
            }

            buttonComp.InitializeButton(cardInfo[i], i, this);

            _cardAnims.Add(animComp);
            _buttons.Add(buttonComp);
            if (i == 0) animComp.InitializeCardForAnim(_spots[(int)EnumMulliganPos.center], EnumMulliganPos.center);
            else if (i == 1) animComp.InitializeCardForAnim(_spots[(int)EnumMulliganPos.rightcenter], EnumMulliganPos.rightcenter);
            else if (i == 2) animComp.InitializeCardForAnim(_spots[(int)EnumMulliganPos.right], EnumMulliganPos.right);
            else animComp.InitializeCardForAnim(_spots[(int)EnumMulliganPos.rightout], EnumMulliganPos.rightout);
        }
    }

    public override void InteractWithScrollCard(string cardName, UI_ScrollCardButton pressed)
    {
        base.InteractWithScrollCard(cardName, pressed);

        if((pressed as UI_GraveyardScrollCard).CardData.cardEffects.Contains(EnumCardEffects.Agile))
        {
            m_agileSelector.ShowScreen((pressed as UI_GraveyardScrollCard).CardData.unitPlacement);
            return;
        }

        m_interactButton.gameObject.SetActive(true);
    }

    public void HideInteractButton()
    {
        m_interactButton.gameObject.SetActive(false);
    }

    private void ShowInteractButton(EnumUnitPlacement placement)
    {
        m_interactButton.gameObject.SetActive(true);
        _cardPlace = placement;
    }
}
