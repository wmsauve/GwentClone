# GwentClone
Just for fun.

# TODO
## Card Order
Move card order in hierarchy to render moused over card on top. Put card back in original order to move down to the "not moused over" position.
## Playing Cards
Final missing thing from Playing a decoy card is swapping the card in the user's hand with the card they swapped.

# Known Major Bugs
## Mulligan bug
Sometimes, when you scroll over and mulligan a card, it will throw invalid input to server error.
## Not resetting outline
After leaving dragging over single target card (like using Decoy), the outline doesn't go away.
## Score not resetting when cards are destroyed.
Especially when scorch is used, the card will be removed, but server is not recalculating score correctly.