using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace AyahaGraphicDevelopTools.ParticlePropertiesClear
{
    public class ParticleUnusedPropertiesClear : EditorWindow
    {
        /// <summary> _serializedObject </summary>
        private SerializedObject _serializedObject;

        /// <summary> 変更するPrefabのList </summary>
        [SerializeField] private List<GameObject> _particleObjs;

        /// <summary> 変更するPrefabのListのSerializedProperty </summary>
        private SerializedProperty _particleObjsProperty;
        
        /// <summary>
        /// ウィンドウを出す
        /// </summary>
        [MenuItem("AyahaGraphicDevelopTools/ParticleUnusedPropertiesClear")]
        public static void ShowWindow()
        {
            var window = GetWindow<ParticleUnusedPropertiesClear>("ParticleUnusedPropertiesClear");
            window.titleContent = new GUIContent("ParticleUnusedPropertiesClear");
        }
        
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _particleObjsProperty = _serializedObject.FindProperty("_particleObjs");
        }

        private void OnGUI()
        {
            _serializedObject.Update();

            ViewTargetPrefabList();

            ViewUnusedPropertiesClearButton();
            
            _serializedObject.ApplyModifiedProperties();
        }
        
        /// <summary>
        /// 対象となるPrefabのList
        /// </summary>
        private void ViewTargetPrefabList()
        {
            EditorGUILayout.PropertyField(_particleObjsProperty, new GUIContent("対象Prefabリスト"), true);
        }

        /// <summary>
        /// 不必要なプロパティを消すボタン
        /// </summary>
        private void ViewUnusedPropertiesClearButton()
        {
            if (GUILayout.Button("消す"))
            {
                DeleteUnusedParticleProperties();
            }
        }
        
        private void DeleteUnusedParticleProperties()
        {
            if (_particleObjsProperty == null || _particleObjsProperty.arraySize == 0)
            {
                return;
            }

            for (int i = 0; i < _particleObjsProperty.arraySize; i++)
            {
                SerializedProperty element = _particleObjsProperty.GetArrayElementAtIndex(i);
                GameObject prefab = element.objectReferenceValue as GameObject;
                if (prefab == null)
                {
                    continue;
                }
                
                string path = AssetDatabase.GetAssetPath(prefab);
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);

                // パーティクルシステム持ってなかったらContinue
                var particleSystems = prefabRoot.GetComponentsInChildren<ParticleSystem>(true);
                if (particleSystems == null || particleSystems.Length == 0)
                {
                    continue;
                }

                for (int j = 0; j < particleSystems.Length; j++)
                {
                    ClearShapeModuleProperties(particleSystems[j]);
                    ClearExternalForcesModuleProperties(particleSystems[j]);
                    ClearCollisionModuleProperties(particleSystems[j]);
                    ClearTriggerModuleProperties(particleSystems[j]);
                    ClearSubEmittersModuleProperties(particleSystems[j]);
                    ClearTextureSheetAnimationModuleProperties(particleSystems[j]);
                    ClearLightModuleProperties(particleSystems[j]);
                    ClearRenderModuleProperties(particleSystems[j]);
                }
                
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
                PrefabUtility.UnloadPrefabContents(prefabRoot);
                
                Debug.Log("A");
            }
        }

        /// <summary>
        /// Shapeモジュールから不要なプロパティを消す
        /// </summary>
        /// <param name="particleSystem"></param>
        private void ClearShapeModuleProperties(ParticleSystem particleSystem)
        {
            var shapeModule = particleSystem.shape;
            
            // shapeModuleが使用されていなかったら関係するアセット参照を消す
            if (!shapeModule.enabled)
            {
                shapeModule.texture = null;
                shapeModule.mesh = null;
                shapeModule.meshRenderer = null;
                shapeModule.skinnedMeshRenderer = null;
                shapeModule.sprite = null;
                shapeModule.spriteRenderer = null;
                return;
            }
            
            // 対応したMesh以外にアセット参照がある可能性があるので消す
            switch (shapeModule.shapeType)
            {
                case ParticleSystemShapeType.Mesh:
                    shapeModule.meshRenderer = null;
                    shapeModule.skinnedMeshRenderer = null;
                    shapeModule.sprite = null;
                    shapeModule.spriteRenderer = null;
                    break;

                case ParticleSystemShapeType.MeshRenderer:
                    shapeModule.mesh = null;
                    shapeModule.skinnedMeshRenderer = null;
                    shapeModule.sprite = null;
                    shapeModule.spriteRenderer = null;
                    break;

                case ParticleSystemShapeType.SkinnedMeshRenderer:
                    shapeModule.mesh = null;
                    shapeModule.meshRenderer = null;
                    shapeModule.sprite = null;
                    shapeModule.spriteRenderer = null;
                    break;

                case ParticleSystemShapeType.Sprite:
                    shapeModule.mesh = null;
                    shapeModule.meshRenderer = null;
                    shapeModule.skinnedMeshRenderer = null;
                    shapeModule.spriteRenderer = null;
                    break;

                case ParticleSystemShapeType.SpriteRenderer:
                    shapeModule.mesh = null;
                    shapeModule.meshRenderer = null;
                    shapeModule.skinnedMeshRenderer = null;
                    shapeModule.sprite = null;
                    break;

                default:
                    // どれにも一致しない場合はすべて除去
                    shapeModule.mesh = null;
                    shapeModule.meshRenderer = null;
                    shapeModule.skinnedMeshRenderer = null;
                    shapeModule.sprite = null;
                    shapeModule.spriteRenderer = null;
                    break;
            }
        }

        /// <summary>
        /// ExternalForcesモジュールから不要なプロパティを消す
        /// </summary>
        /// <param name="particleSystem"></param>
        private void ClearExternalForcesModuleProperties(ParticleSystem particleSystem)
        {
            var externalForcesModule = particleSystem.externalForces;
            
            // 使用されていなかったら関係するアセット参照を消す
            if (!externalForcesModule.enabled)
            {
                externalForcesModule.RemoveAllInfluences();
                return;
            }

            // influenceFilterがListかLayerMaskAndListでなければ参照を消す
            if (externalForcesModule.influenceFilter != ParticleSystemGameObjectFilter.List || externalForcesModule.influenceFilter != ParticleSystemGameObjectFilter.LayerMaskAndList)
            {
                externalForcesModule.RemoveAllInfluences();
            }
        }

        /// <summary>
        /// Collisionモジュールから不要なプロパティを消す
        /// </summary>
        /// <param name="particleSystem"></param>
        private void ClearCollisionModuleProperties(ParticleSystem particleSystem)
        {
            var collisionModule = particleSystem.collision;

            // 全てのPlaneを消す
            void RemoveAllPlane()
            {
                var planeCount = collisionModule.planeCount;
                if (planeCount > 0)
                {
                    for (int i = 0; i < planeCount; i++)
                    {
                        collisionModule.RemovePlane(i);
                    }
                }
            }
            
            // 使用されていなかったら関係するアセット参照を消す
            if (!collisionModule.enabled)
            {
                RemoveAllPlane();
                return;
            }

            // CollisionTypeがPlanesでなければPlaneを消す
            if (collisionModule.type != ParticleSystemCollisionType.Planes)
            {
                RemoveAllPlane();
            }
        }

        /// <summary>
        /// Triggerモジュールから不要なプロパティを消す
        /// </summary>
        /// <param name="particleSystem"></param>
        private void ClearTriggerModuleProperties(ParticleSystem particleSystem)
        {
            var triggerModule = particleSystem.trigger;
            
            // 全てのコライダーを消す
            void RemoveAllCollider()
            {
                var colliderCount = triggerModule.colliderCount;
                if (colliderCount > 0)
                {
                    for (int i = 0; i < colliderCount; i++)
                    {
                        triggerModule.RemoveCollider(i);
                    }
                }
            }

            // 使用されていなかったら関係するアセットを消す
            if (!triggerModule.enabled)
            {
                RemoveAllCollider();
            }
        }

        /// <summary>
        /// SubEmitterモジュールから不要なプロパティを消す
        /// </summary>
        /// <param name="particleSystem"></param>
        private void ClearSubEmittersModuleProperties(ParticleSystem particleSystem)
        {
            var subEmitterModule = particleSystem.subEmitters;

            // 全てのサブエミッターを消す
            void RemoveAllSubEmitter()
            {
                var subEmittersCount = subEmitterModule.subEmittersCount;
                if (subEmittersCount > 0)
                {
                    for (int i = 0; i < subEmittersCount; i++)
                    {
                        subEmitterModule.RemoveSubEmitter(i);
                    }
                }
            }
            
            // 使用されていなかったら関係するアセットを消す
            if (!subEmitterModule.enabled)
            {
                RemoveAllSubEmitter();
            }
        }

        /// <summary>
        /// TextureSheetAnimationモジュールから不要なプロパティを消す
        /// </summary>
        /// <param name="particleSystem"></param>
        private void ClearTextureSheetAnimationModuleProperties(ParticleSystem particleSystem)
        {
            var textureSheetAnimationModule = particleSystem.textureSheetAnimation;

            // 全てのスプライトを消す
            void RemoveAllSubSprite()
            {
                var subEmittersCount = textureSheetAnimationModule.spriteCount;
                if (subEmittersCount > 0)
                {
                    for (int i = 0; i < subEmittersCount; i++)
                    {
                        textureSheetAnimationModule.RemoveSprite(i);
                    }
                }
            }
            
            // 使用されていなかったら関係するアセットを消す
            if (!textureSheetAnimationModule.enabled)
            {
                RemoveAllSubSprite();
                return;
            }
            
            if (textureSheetAnimationModule.mode != ParticleSystemAnimationMode.Sprites)
            {
                RemoveAllSubSprite();
            }
        }

        /// <summary>
        /// Lightモジュールから不要なプロパティを消す
        /// </summary>
        /// <param name="particleSystem"></param>
        private void ClearLightModuleProperties(ParticleSystem particleSystem)
        {
            var lightModule = particleSystem.lights;

            // 使用されていなかったら関係するアセットを消す
            if (!lightModule.enabled)
            {
                lightModule.light = null;
            }
        }
        
        /// <summary>
        /// Rendererモジュールから不要なプロパティを消す
        /// </summary>
        /// <param name="particleSystem"></param>
        private void ClearRenderModuleProperties(ParticleSystem particleSystem)
        {
            var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                // そもそもモジュールを使用していなかったら関係するアセット参照を消す
                if (!renderer.enabled)
                {
                    renderer.material = null;
                    renderer.trailMaterial = null;
                    renderer.SetMeshes(Array.Empty<Mesh>());
                    renderer.probeAnchor = null;
                    return;
                }
                
                // RenderModeがMeshでなければMeshへの参照を消す
                if (renderer.renderMode != ParticleSystemRenderMode.Mesh)
                {
                    renderer.SetMeshes(Array.Empty<Mesh>());
                }

                // LightProbeUsageがBlendProbesでなければprobeAnchorの参照を消す
                if (renderer.lightProbeUsage != LightProbeUsage.BlendProbes)
                {
                    renderer.probeAnchor = null;
                }
            }
        }
    }
}
