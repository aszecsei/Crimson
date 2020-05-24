namespace Crimson.Physics
{
    public class PhysicsSubsystem : Subsystem
    {
        private const int TARGET_PHYSICS_FRAMERATE = 50;
        private float _elapsedTime = 0f;
        protected internal override void Tick()
        {
            base.Tick();
            
            // Update the elapsed time
            _elapsedTime += Time.RawDeltaTime;

            while (_elapsedTime >= 1000f / TARGET_PHYSICS_FRAMERATE)
            {
                DoPhysics();
                _elapsedTime -= (1000f / TARGET_PHYSICS_FRAMERATE);
            }
        }

        private void DoPhysics()
        {
            
        }
    }
}