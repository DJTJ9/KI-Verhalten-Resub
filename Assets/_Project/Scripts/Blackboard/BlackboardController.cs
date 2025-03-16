using Sirenix.OdinInspector;
using UnityEngine;

namespace BlackboardSystem {
    public class BlackboardController : MonoBehaviour {
        [InlineEditor, SerializeField] BlackboardData blackboardData;
        readonly Blackboard blackboard = new Blackboard();

        void Awake() {
            blackboardData.SetValuesOnBlackboard(blackboard);
            blackboard.Debug();
        }
        
        public Blackboard GetBlackboard() => blackboard;
    }
}