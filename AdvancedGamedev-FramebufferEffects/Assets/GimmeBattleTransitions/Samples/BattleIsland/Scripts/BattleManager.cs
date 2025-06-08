using UnityEngine;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace GimmeBattleTransitions.Samples
{
    public class BattleManager : MonoBehaviour
    {

#if UNITY_EDITOR

        public SceneAsset battleScene;
        public SceneAsset battleIslandScene;

#endif

        #region Private Fields

        private bool sceneWasChanged = false;
        private bool hasSavedPositions = false;

        private int lastTransition = -1;

        private Matrix4x4 savedCharacterPosition;
        private Matrix4x4 savedCameraPosition;

        #endregion

        public bool HasSavedPositions() => this.hasSavedPositions;
        public bool SceneWasChanged() => this.sceneWasChanged;

        public int GetLastTransition() => this.lastTransition;
        public Matrix4x4 GetSavedCharacterPosition() => this.savedCharacterPosition;
        public Matrix4x4 GetSavedCameraPosition() => this.savedCameraPosition;  

        private void Awake()
        {
            var managers = GameObject.FindObjectsByType<BattleManager>(FindObjectsSortMode.None);
            if (managers.Length > 1)
            {
                GameObject.Destroy(this);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }

        public void LoadBattleScene(Matrix4x4 characterPosition, Matrix4x4 cameraPosition, int lastTransition)
        {
            this.hasSavedPositions = true;
            this.sceneWasChanged = true;
            this.savedCharacterPosition = characterPosition;
            this.savedCameraPosition = cameraPosition;
            this.lastTransition = lastTransition;

#if UNITY_EDITOR
            var scenePath = AssetDatabase.GetAssetPath(this.battleScene);
            var scene = EditorSceneManager.LoadSceneInPlayMode(scenePath, new LoadSceneParameters());
#else
            var scene = SceneManager.LoadScene("Battle", new LoadSceneParameters());
#endif
            
        }

        public void LoadBattleIslandScene()
        {
            this.sceneWasChanged = true;

#if UNITY_EDITOR
            var scenePath = AssetDatabase.GetAssetPath(this.battleIslandScene);
            var scene = EditorSceneManager.LoadSceneInPlayMode(scenePath, new LoadSceneParameters());
#else
            var scene = SceneManager.LoadScene("BattleIsland", new LoadSceneParameters());
#endif
        }
    }
}
