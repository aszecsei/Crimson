# Utility Based AI

Utility Theory for games. The most complex of the AI solutions. Best used in very dynamic environments where its scoring system works best. Utility based AI are more appropriate in situations where there are a large number of potentially competing actions the AI can take such as in a RTS. A great overview of utility AI is [available here](http://www.gdcvault.com/play/1012410/Improving-AI-Decision-Modeling-Through).


## Reasoner
Selects the best Consideration from a list of Considerations attached to the Reasoner. The root of a utility AI.


## Consideration
Houses a list of Appraisals and an Action. Calculates a score that represents numerically the utility of its Action.


## Appraisal
One or more Appraisals can be added to a Consideration. They calculate and return a score which is used by the Consideration.


## Action
The action that the AI executes when a specific Consideration is selected.