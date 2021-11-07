#region Info
// -----------------------------------------------------------------------
// CM_Worker.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
/*
namespace CM_TaskSystem {

    public class CM_Worker : CM_IWorker {

        private static GridPathfindingSystem.GridPathfinding pathfinding;

        public GameObject gameObject;
        private V_Object unitObject;

        public static CM_Worker Create(Vector3 position) {
            if (pathfinding == null) {
                float nodeSize = 5f;
                Vector3 pathfindingOriginRoam = new Vector3(200, 200);
                Vector3 pathfindingUpperRight = new Vector3(1000, 1000);
                pathfinding = new GridPathfindingSystem.GridPathfinding(pathfindingOriginRoam, pathfindingUpperRight, nodeSize);
            }

            return new CM_Worker(position);
        }

        private CM_Worker(Vector3 position) {
            unitObject = V_GameHandlerLogic_BattleRoyaleTycoon.CreateBasicUnit(position, 30f, SpriteHolder.instance.m_PeasantSpriteSheet, UnitAnimTypeEnum.dBareHands_Walk, UnitAnimTypeEnum.dBareHands_Idle);
            
            V_ObjectLogic_PathfindingHandler pathfindingHandler = unitObject.GetLogic<V_ObjectLogic_PathfindingHandler>();
            pathfindingHandler.SetActivePathfinding(pathfinding);

            gameObject = unitObject.GetLogic<V_IObjectTransform>().GetTransform().gameObject;
        }

        public void MoveTo(Vector3 position, Action onArrivedAtPosition = null) {
            unitObject.GetLogic<V_ObjectWalkerAnimated>().MoveTo(position, onArrivedAtPosition);
        }

        public void PlayVictoryAnimation(Action onAnimationComplete) {
            unitObject.GetLogic<V_UnitAnimation>().PlayAnimForced(BattleRoyaleTycoon.UnitAnimEnum.dBareHands_Victory, 1f, () => {
                unitObject.GetLogic<V_ObjectWalkerAnimated>().PlayAnimIdle();
                onAnimationComplete();
            });
        }

        public void PlayCleanUpAnimation(Action onAnimationComplete) {
            unitObject.GetLogic<V_UnitAnimation>().PlayAnimForced(BattleRoyaleTycoon.UnitAnimEnum.Worker_BroomCleanDownLeft, 1f, () => {
                unitObject.GetLogic<V_UnitAnimation>().PlayAnimForced(BattleRoyaleTycoon.UnitAnimEnum.Worker_BroomCleanDownLeft, 1f, () => {
                    unitObject.GetLogic<V_UnitAnimation>().PlayAnimForced(BattleRoyaleTycoon.UnitAnimEnum.Worker_BroomCleanDownLeft, 1f, () => {
                        unitObject.GetLogic<V_ObjectWalkerAnimated>().PlayAnimIdle();
                        onAnimationComplete();
                    });
                });
            });
        }

        public bool IsMoving() {
            return unitObject.GetLogic<V_ObjectWalkerAnimated>().IsMoving();
        }

        public Vector3 GetPosition() {
            return unitObject.GetPosition();
        }

    }

}
*/

