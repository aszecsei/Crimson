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
            var planner = new TaskPlanner(new ExecuteTask("SingleTask"));
            var plan = planner.Plan(new Blackboard());
            plan.Should().OnlyContain((x) => x == planner["SingleTask"]);
        }

        [Test]
        public void CompoundPlan()
        {
            var rootTask = new CompoundTask("Attack Enemy")
            {
                new Method(new FunctionConditional(x => x.Get<bool>("hasTreeTrunk"))) { "NavigateToEnemy", "DoTrunkSlam" },
                new Method(new FunctionConditional(x => !x.Get<bool>("hasTreeTrunk"))) { "LiftBoulderFromGround", "ThrowBoulderAtEnemy" }
            };
            var planner = new TaskPlanner(rootTask)
            {
                new ExecuteTask("NavigateToEnemy"),
                new ExecuteTask("DoTrunkSlam"),
                new ExecuteTask("LiftBoulderFromGround"),
                new ExecuteTask("ThrowBoulderAtEnemy")
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
                var rootTask = new CompoundTask("BeTrunkThumper")
                {
                    new Method(new FunctionConditional(x => x.Get<bool>("canSeeEnemy"))) { "AttackEnemy" },
                    new Method() { "ChooseBridgeToCheck", "NavigateToBridge", "CheckBridge" }
                };
                var planner = new TaskPlanner(rootTask)
                {
                    new CompoundTask("AttackEnemy")
                    {
                        new Method(new FunctionConditional(x => x.Get<int>("trunkHealth") > 0)) { "NavigateToEnemy", "DoTrunkSlam" },
                        new Method() { "FindTrunk", "NavigateToTrunk", "UprootTrunk", "AttackEnemy" }
                    },
                    new ExecuteTask("DoTrunkSlam", x => x.Set("trunkHealth", x.Get<int>("trunkHealth") - 1)),
                    new ExecuteTask("UprootTrunk", x => x.Set("trunkHealth", 3)),
                    new ExecuteTask("NavigateToTrunk"),
                    new ExecuteTask("ChooseBridgeToCheck"),
                    new ExecuteTask("NavigateToBridge", cost: 5),
                    new ExecuteTask("NavigateToEnemy"),
                    new ExecuteTask("CheckBridge"),
                    new ExecuteTask("FindTrunk"),
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
                var rootTask = new CompoundTask("BeTrunkThumper")
                {
                    new Method(new FunctionConditional(x => x.Get<bool>("canSeeEnemy"))) { "AttackEnemy" },
                    new Method() { "ChooseBridgeToCheck", "NavigateToBridge", "CheckBridge" }
                };
                var planner = new TaskPlanner(rootTask)
                {
                    new CompoundTask("AttackEnemy")
                    {
                        new Method(new FunctionConditional(x => x.Get<int>("trunkHealth") > 0)) { "NavigateToEnemy", "DoTrunkSlam" },
                        new Method() { "FindTrunk", "NavigateToTrunk", "UprootTrunk", "AttackEnemy" }
                    },
                    new ExecuteTask("DoTrunkSlam", x => x.Set("trunkHealth", x.Get<int>("trunkHealth") - 1)),
                    new ExecuteTask("UprootTrunk", x => x.Set("trunkHealth", 3)),
                    new ExecuteTask("NavigateToTrunk"),
                    new ExecuteTask("ChooseBridgeToCheck"),
                    new ExecuteTask("NavigateToBridge", cost: 5),
                    new ExecuteTask("NavigateToEnemy"),
                    new ExecuteTask("CheckBridge"),
                    new ExecuteTask("FindTrunk"),
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
                var rootTask = new CompoundTask("BeTrunkThumper")
                {
                    new Method(new FunctionConditional(x => x.Get<bool>("canSeeEnemy"))) { "AttackEnemy" },
                    new Method() { "ChooseBridgeToCheck", "NavigateToBridge", "CheckBridge" }
                };
                var planner = new TaskPlanner(rootTask)
                {
                    new CompoundTask("AttackEnemy")
                    {
                        new Method(new FunctionConditional(x => x.Get<int>("trunkHealth") > 0)) { "NavigateToEnemy", "DoTrunkSlam" },
                        new Method() { "FindTrunk", "NavigateToTrunk", "UprootTrunk", "AttackEnemy" }
                    },
                    new ExecuteTask("DoTrunkSlam", x => x.Set("trunkHealth", x.Get<int>("trunkHealth") - 1)),
                    new ExecuteTask("UprootTrunk", x => x.Set("trunkHealth", 3)),
                    new ExecuteTask("NavigateToTrunk"),
                    new ExecuteTask("ChooseBridgeToCheck"),
                    new ExecuteTask("NavigateToBridge", cost: 5),
                    new ExecuteTask("NavigateToEnemy"),
                    new ExecuteTask("CheckBridge"),
                    new ExecuteTask("FindTrunk"),
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