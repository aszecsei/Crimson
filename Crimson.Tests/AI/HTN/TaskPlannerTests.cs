using Crimson.AI;
using Crimson.AI.HTN;
using FluentAssertions;
using NUnit.Framework;

namespace Crimson.Tests.AI.HTN
{
    [TestFixture]
    public class TaskPlannerTests
    {
        [Test]
        public void BasicPlan()
        {
            var planner = new TaskPlanner(new PrimitiveTask<Blackboard>("SingleTask"));
            var plan = planner.Plan(new Blackboard());
            plan.Should().OnlyContain((x) => x == planner["SingleTask"]);
        }

        [Test]
        public void CompoundPlan()
        {
            var rootTask = new CompoundTask<Blackboard>("Attack Enemy")
            {
                new Method<Blackboard>(new FunctionConditional<Blackboard>(x => x.Get<bool>("hasTreeTrunk"))) { "NavigateToEnemy", "DoTrunkSlam" },
                new Method<Blackboard>(new FunctionConditional<Blackboard>(x => !x.Get<bool>("hasTreeTrunk"))) { "LiftBoulderFromGround", "ThrowBoulderAtEnemy" }
            };
            var planner = new TaskPlanner(rootTask)
            {
                new PrimitiveTask<Blackboard>("NavigateToEnemy"),
                new PrimitiveTask<Blackboard>("DoTrunkSlam"),
                new PrimitiveTask<Blackboard>("LiftBoulderFromGround"),
                new PrimitiveTask<Blackboard>("ThrowBoulderAtEnemy")
            };
            
            Blackboard b = new Blackboard();
            b.Set("hasTreeTrunk", false);
            var plan = planner.Plan(b);
            plan.Should().HaveCount(2);
            plan.Should().Contain(x => x.Name == "LiftBoulderFromGround");
            plan.Should().Contain(x => x.Name == "ThrowBoulderAtEnemy");
            
            b.Set("hasTreeTrunk", true);
            plan = planner.Plan(b);
            plan.Should().HaveCount(2);
            plan.Should().Contain(x => x.Name == "NavigateToEnemy");
            plan.Should().Contain(x => x.Name == "DoTrunkSlam");
        }

        [TestFixture]
        public class Recursion
        {
            [Test]
            public void CannotSeeEnemy()
            {
                var rootTask = new CompoundTask<Blackboard>("BeTrunkThumper")
                {
                    new Method<Blackboard>(new FunctionConditional<Blackboard>(x => x.Get<bool>("canSeeEnemy"))) { "AttackEnemy" },
                    new Method<Blackboard>() { "ChooseBridgeToCheck", "NavigateToBridge", "CheckBridge" }
                };
                var planner = new TaskPlanner(rootTask)
                {
                    new CompoundTask<Blackboard>("AttackEnemy")
                    {
                        new Method<Blackboard>(new FunctionConditional<Blackboard>(x => x.Get<int>("trunkHealth") > 0)) { "NavigateToEnemy", "DoTrunkSlam" },
                        new Method<Blackboard>() { "FindTrunk", "NavigateToTrunk", "UprootTrunk", "AttackEnemy" }
                    },
                    new ExecuteTask<Blackboard>("DoTrunkSlam", x => x.Set("trunkHealth", x.Get<int>("trunkHealth") - 1)),
                    new ExecuteTask<Blackboard>("UprootTrunk", x => x.Set("trunkHealth", 3)),
                    new PrimitiveTask<Blackboard>("NavigateToTrunk"),
                    new PrimitiveTask<Blackboard>("ChooseBridgeToCheck"),
                    new PrimitiveTask<Blackboard>("NavigateToBridge", 5),
                    new PrimitiveTask<Blackboard>("NavigateToEnemy"),
                    new PrimitiveTask<Blackboard>("CheckBridge"),
                    new PrimitiveTask<Blackboard>("FindTrunk"),
                };
            
                var b = new Blackboard();
                b.Set("canSeeEnemy", false);
                b.Set("trunkHealth", 1);
            
                var plan = planner.Plan(b);
                plan.Should().HaveCount(3);
                plan.Should().Contain(x => x.Name == "ChooseBridgeToCheck");
                plan.Should().Contain(x => x.Name == "NavigateToBridge");
                plan.Should().Contain(x => x.Name == "CheckBridge");
            }

            [Test]
            public void CanSeeEnemyTrunkHealthy()
            {
                var rootTask = new CompoundTask<Blackboard>("BeTrunkThumper")
                {
                    new Method<Blackboard>(new FunctionConditional<Blackboard>(x => x.Get<bool>("canSeeEnemy"))) { "AttackEnemy" },
                    new Method<Blackboard>() { "ChooseBridgeToCheck", "NavigateToBridge", "CheckBridge" }
                };
                var planner = new TaskPlanner(rootTask)
                {
                    new CompoundTask<Blackboard>("AttackEnemy")
                    {
                        new Method<Blackboard>(new FunctionConditional<Blackboard>(x => x.Get<int>("trunkHealth") > 0)) { "NavigateToEnemy", "DoTrunkSlam" },
                        new Method<Blackboard>() { "FindTrunk", "NavigateToTrunk", "UprootTrunk", "AttackEnemy" }
                    },
                    new ExecuteTask<Blackboard>("DoTrunkSlam", x => x.Set("trunkHealth", x.Get<int>("trunkHealth") - 1)),
                    new ExecuteTask<Blackboard>("UprootTrunk", x => x.Set("trunkHealth", 3)),
                    new PrimitiveTask<Blackboard>("NavigateToTrunk"),
                    new PrimitiveTask<Blackboard>("ChooseBridgeToCheck"),
                    new PrimitiveTask<Blackboard>("NavigateToBridge", 5),
                    new PrimitiveTask<Blackboard>("NavigateToEnemy"),
                    new PrimitiveTask<Blackboard>("CheckBridge"),
                    new PrimitiveTask<Blackboard>("FindTrunk"),
                };
            
                var b = new Blackboard();
                b.Set("canSeeEnemy", true);
                b.Set("trunkHealth", 1);
                
                var plan = planner.Plan(b);
                plan.Should().HaveCount(2);
                plan.Should().Contain(x => x.Name == "NavigateToEnemy");
                plan.Should().Contain(x => x.Name == "DoTrunkSlam");
            }

            [Test]
            public void CanSeeEnemyTrunkGone()
            {
                var rootTask = new CompoundTask<Blackboard>("BeTrunkThumper")
                {
                    new Method<Blackboard>(new FunctionConditional<Blackboard>(x => x.Get<bool>("canSeeEnemy"))) { "AttackEnemy" },
                    new Method<Blackboard>() { "ChooseBridgeToCheck", "NavigateToBridge", "CheckBridge" }
                };
                var planner = new TaskPlanner(rootTask)
                {
                    new CompoundTask<Blackboard>("AttackEnemy")
                    {
                        new Method<Blackboard>(new FunctionConditional<Blackboard>(x => x.Get<int>("trunkHealth") > 0)) { "NavigateToEnemy", "DoTrunkSlam" },
                        new Method<Blackboard>() { "FindTrunk", "NavigateToTrunk", "UprootTrunk", "AttackEnemy" }
                    },
                    new ExecuteTask<Blackboard>("DoTrunkSlam", x => x.Set("trunkHealth", x.Get<int>("trunkHealth") - 1)),
                    new ExecuteTask<Blackboard>("UprootTrunk", x => x.Set("trunkHealth", 3)),
                    new PrimitiveTask<Blackboard>("NavigateToTrunk"),
                    new PrimitiveTask<Blackboard>("ChooseBridgeToCheck"),
                    new PrimitiveTask<Blackboard>("NavigateToBridge", 5),
                    new PrimitiveTask<Blackboard>("NavigateToEnemy"),
                    new PrimitiveTask<Blackboard>("CheckBridge"),
                    new PrimitiveTask<Blackboard>("FindTrunk"),
                };
            
                var b = new Blackboard();
                b.Set("canSeeEnemy", true);
                b.Set("trunkHealth", 0);
                
                var plan = planner.Plan(b);
                plan.Should().HaveCount(5);
                plan.Should().Contain(x => x.Name == "FindTrunk");
                plan.Should().Contain(x => x.Name == "NavigateToTrunk");
                plan.Should().Contain(x => x.Name == "UprootTrunk");
                plan.Should().Contain(x => x.Name == "NavigateToEnemy");
                plan.Should().Contain(x => x.Name == "DoTrunkSlam");
            }
        }
    }
}