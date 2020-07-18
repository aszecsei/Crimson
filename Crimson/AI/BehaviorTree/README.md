# Behavior Trees

The de facto standard for composing AI for the last decade. Behavior trees are composed of a tree of nodes. Nodes can make decisions and perform actions based on the state of the world. Crimson includes a `BehaviorTreeBuilder` class that provides a fluent API for setting up a behavior tree. The `BehaviorTreeBuilder` is a great way to reduce the barrier of entry to using behavior trees and get up and running quickly.


## Composites
Composites are parent nodes in a behavior tree. They house 1 or more children and execute them in different ways.

- **Sequence<T>:** returns failure as soon as one of its children returns failure. If a child returns success it will sequentially run the next child in the next tick of the tree.
- **Selector<T>:** returns success as soon as one of its child tasks return success. If a child task returns failure then it will sequentially run the next child in the next tick.
- **Parallel<T>:** runs each child until a child returns failure. It differs from `Sequence` only in that it runs all children every tick
- **ParallelSelector<T>:** like a `Selector` except it will run all children every tick
- **RandomSequence<T>:** a `Sequence` that shuffles its children before executing
- **RandomSelector<T>:** a `Selector` that shuffles its children before executing
- **UtilitySelector<T>:** a `Selector` that orders its children by utility before executing

## Conditionals
Conditionals are binary success/failure nodes. They are identified by the IConditional interface. They check some condition of your game world and either return success or failure. These are inherently game specific so Crimson only provides a single generic Conditional out of the box and a helper Conditional that wraps an Action so you can avoid having to make a separate class for each Conditional.

- **RandomProbability<T>:** return success when the random probability is above the specified success probability
- **ExecuteActionConditional<T>:** wraps a Func and executes it as the Conditional. Useful for prototyping and to avoid creating separate classes for simple Conditionals.


## Decorators
Decorators are wrapper tasks that have a single child. They can modify the behavior of the child task in various ways such as inverting the result, running it until failure, etc.

- **AlwaysFail<T>:** always returns failure regardless of the child result
- **AlwaysSucceed<T>:** always returns success regardless of the child result
- **ConditionalDecorator<T>:** wraps a Conditional and will only run its child if a condition is met
- **Inverter<T>:** inverts the result of its child
- **Repeater<T>:** repeats its child task a specified number of times
- **UntilFail<T>:** keeps executing its child task until it returns failure
- **UntilSuccess<T>:** keeps executing its child task until it returns success


## Actions
Actions are the leaf nodes of the behavior tree. This is where stuff happens such as playing an animation, triggering an event, etc.

- **ExecuteAction<T>:** wraps a Func and executes it as its action. Useful for prototyping and to avoid creating separate classes for simple Actions.
- **WaitAction<T>:** waits a specified amount of time
- **LogAction<T>:** logs a string to the console. Useful for debugging.
- **BehaviorTreeReference<T>:** runs another BehaviorTree<T>