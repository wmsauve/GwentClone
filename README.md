# GwentClone
Just for fun.

# TODO
## Card Order
Move card order in hierarchy to render moused over card on top. Put card back in original order to move down to the "not moused over" position.
## Playing Cards
For Spy, need to only highlight your own cards. Need to handle logic on server to swap played card with spy card.

# Known Major Bugs
## Mulligan bug
Sometimes, when you scroll over and mulligan a card, it will throw invalid input to server error.
## Not resetting outline
After dropping card inside zone, outline doesn't reset. Similarly, when resetting card being held at end of turn, zone outline doesn't reset.