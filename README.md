# GwentClone
Just for fun.

# TODO
## Card Order
Move card order in hierarchy to render moused over card on top. Put card back in original order to move down to the "not moused over" position.
## Playing Cards
Ensure all cards highlight the correct zones whenever you are dragging them to play them in the game zone.
Specifically need to add highlighting for global cards.
Also need to be considerate of state management with player controls. Refactor to separate allowable controls during player turn from opponent turn.

# Known Major Bugs
## Mulligan bug
Sometimes, when you scroll over and mulligan a card, it will throw invalid input to server error.
## End Turn With Card Holding
After the most recent refactoring with player controls, need to refactor turn ending reseting held card back to hand.