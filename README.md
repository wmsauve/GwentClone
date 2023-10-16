# GwentClone
Just for fun.

# Current Task Being Worked On
To Test: Muster seems to work without having to do an odd ordering to playing cards on server. Test scorch and medic before moving forward.

Work on stat boosting effects. (horn)

# TODO
## Card Order
Move card order in hierarchy to render moused over card on top. Put card back in original order to move down to the "not moused over" position.
## UX
Work on Medic. Need to add functionality to cancel button so that people can decide not to continue playing a medic.
## Server Logic
Playing multi stage cards like medic should be a single client rpc call, not multiple calls in a loop.

# Known Major Bugs
## Mulligan bug
Sometimes, when you scroll over and mulligan a card, it will throw invalid input to server error.
## Playing siege
Sometimes, After you start dragging a siege card, it wont be able to be dropped on the siege zone (probably need to pull camera back).
