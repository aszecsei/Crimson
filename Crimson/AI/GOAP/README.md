# Goal-Oriented Action Planning (GOAP)

GOAP differs quite a bit from the other AI solutions. With GOAP, you provide the planner with a list of the actions that the AI can perform, the current world state and the desired world state (goal state). GOAP will then attempt to find a series of actions that will get the AI to the goal state.

GOAP was made popular by the old FPS F.E.A.R. The AI in F.E.A.R. consisted of a GOAP and a state machine with just 3 states: GoTo, Animate, UseSmartObject. Jeff Orkin's [web page](http://alumni.media.mit.edu/~jorkin/goap.html) is a treasure trove of great information.


## ActionPlanner
The brains of the operation. You give the ActionPlanner all of your Actions, the current world state and your goal state and it will give you back the best possible plan to achieve the goal state.


## Action/ActionT
Actions define a list of pre conditions that they require and a list of post conditions that will occur when the Action is performed. ActionT is just a subclass of Action with a handy context object of type T.


## Agent
Agent is a helper class that encapsulates an AI agent. It keeps a list of available Actions and a reference to the ActionPlanner. Agent is abstract and requires you to define the `GetWorldState` and `GetGoalState` methods. With those in place getting a plan is as simple as calling `agent.Plan()`.