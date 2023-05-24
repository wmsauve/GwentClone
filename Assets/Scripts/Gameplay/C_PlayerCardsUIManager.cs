using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_PlayerCardsUIManager : MonoBehaviour
{
    public GameObject cardPrefab; // Prefab of the card object
    public float cardPadding = 0.2f; // Padding between the cards
    public float availableWidth = 5.0f; // Width of the available space for the hand
    public int testHandSize = 3;

    private void Start()
    {
        int totalCards = testHandSize; // Determine the total number of cards in the hand

        float cardWidth = CalculateCardWidth(totalCards); // Calculate the width of each card
        float startingPosition = CalculateStartingPosition(totalCards, cardWidth); // Calculate the starting position of the first card

        for (int i = 0; i < totalCards; i++)
        {
            GameObject card = Instantiate(cardPrefab, transform); // Instantiate a new card object
            float cardPosition = startingPosition + (i * (cardWidth + cardPadding)); // Calculate the position of the current card
            card.transform.localPosition = new Vector3(cardPosition, 0.0f, 0.0f); // Set the position of the card
        }
    }

    private float CalculateCardWidth(int totalCards)
    {
        float totalPadding = (totalCards - 1) * cardPadding; // Calculate the total padding between the cards
        return (availableWidth - totalPadding) / totalCards; // Calculate the width of each card
    }

    private float CalculateStartingPosition(int totalCards, float cardWidth)
    {
        float totalWidth = totalCards * cardWidth + (totalCards - 1) * cardPadding; // Calculate the total width of all the cards
        return -(totalWidth / 2) + (cardWidth / 2); // Calculate the starting position of the first card
    }

    private void OnTransformChildrenChanged()
    {
        ReadjustCardPositionsInHand();
    }


    private void ReadjustCardPositionsInHand()
    {

    }
}
