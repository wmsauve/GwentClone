# GwentClone
Just for fun.

# Current Task Being Worked On
Work on stat boosting effects. (horn)

# TODO
## Card Order
Move card order in hierarchy to render moused over card on top. Put card back in original order to move down to the "not moused over" position.
## UX
Work on Medic. Need to add functionality to cancel button so that people can decide not to continue playing a medic.
## Server Logic
Playing multi stage cards like medic should be a single client rpc call, not multiple calls in a loop.
## Refactor Code
Create a method to create the card struct that is converted to JSON since the logic will expand. Rename it from CardToClient.

# Known Major Bugs
## Mulligan bug
Sometimes, when you scroll over and mulligan a card, it will throw invalid input to server error.
## Playing siege
Sometimes, After you start dragging a siege card, it wont be able to be dropped on the siege zone (probably need to pull camera back).
