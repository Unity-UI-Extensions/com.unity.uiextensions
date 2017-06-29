/// Credit glennpow
/// Sourced from - http://forum.unity3d.com/threads/free-script-particle-systems-in-ui-screen-space-overlay.406862/
/// *Note - experimental.  Currently renders in scene view and not game view.

namespace UnityEngine.UI.Extensions
{
#if UNITY_5_3_OR_NEWER
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(ParticleSystem))]
    public class UIParticleSystem : MaskableGraphic
    {

        public Texture particleTexture;
        public Sprite particleSprite;

        private Transform _transform;
        private ParticleSystem _particleSystem;
        private ParticleSystem.Particle[] _particles;
        private UIVertex[] _quad = new UIVertex[4];
        private Vector4 _uv = Vector4.zero;
        private ParticleSystem.TextureSheetAnimationModule _textureSheetAnimation;
        private int _textureSheetAnimationFrames;
        private Vector2 _textureSheedAnimationFrameSize;

        public override Texture mainTexture
        {
            get
            {
                if (particleTexture)
                {
                    return particleTexture;
                }

                if (particleSprite)
                {
                    return particleSprite.texture;
                }

                return null;
            }
        }

        protected bool Initialize()
        {
            // initialize members
            if (_transform == null)
            {
                _transform = transform;
            }

            // prepare particle system
            ParticleSystemRenderer renderer = GetComponent<ParticleSystemRenderer>();
            bool setParticleSystemMaterial = false;

            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();

                if (_particleSystem == null)
                {
                    return false;
                }

                // get current particle texture
                if (renderer == null)
                {
                    renderer = _particleSystem.gameObject.AddComponent<ParticleSystemRenderer>();
                }
                Material currentMaterial = renderer.sharedMaterial;
                if (currentMaterial && currentMaterial.HasProperty("_MainTex"))
                {
                    particleTexture = currentMaterial.mainTexture;
                }

                // automatically set scaling
                _particleSystem.scalingMode = ParticleSystemScalingMode.Local;

                _particles = null;
                setParticleSystemMaterial = true;
            }
            else
            {
                if (Application.isPlaying)
                {
                    setParticleSystemMaterial = (renderer.material == null);
                }
#if UNITY_EDITOR
                else
                {
                    setParticleSystemMaterial = (renderer.sharedMaterial == null);
                }
#endif
            }

            // automatically set material to UI/Particles/Hidden shader, and get previous texture
            if (setParticleSystemMaterial)
            {
                Material material = new Material(Shader.Find("UI/Particles/Hidden"));
                if (Application.isPlaying)
                {
                    renderer.material = material;
                }
#if UNITY_EDITOR
                else
                {
                    material.hideFlags = HideFlags.DontSave;
                    renderer.sharedMaterial = material;
                }
#endif
            }

            // prepare particles array
            if (_particles == null)
            {
                _particles = new ParticleSystem.Particle[_particleSystem.maxParticles];
            }

            // prepare uvs
            if (particleTexture)
            {
                _uv = new Vector4(0, 0, 1, 1);
            }
            else if (particleSprite)
            {
                _uv = UnityEngine.Sprites.DataUtility.GetOuterUV(particleSprite);
            }

            // prepare texture sheet animation
            _textureSheetAnimation = _particleSystem.textureSheetAnimation;
            _textureSheetAnimationFrames = 0;
            _textureSheedAnimationFrameSize = Vector2.zero;
            if (_textureSheetAnimation.enabled)
            {
                _textureSheetAnimationFrames = _textureSheetAnimation.numTilesX * _textureSheetAnimation.numTilesY;
                _textureSheedAnimationFrameSize = new Vector2(1f / _textureSheetAnimation.numTilesX, 1f / _textureSheetAnimation.numTilesY);
            }

            return true;
        }

        protected override void Awake()
        {
            base.Awake();

            if (!Initialize())
            {
                enabled = false;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (!Initialize())
                {
                    return;
                }
            }
#endif

            // prepare vertices
            vh.Clear();

            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            // iterate through current particles
            int count = _particleSystem.GetParticles(_particles);

            for (int i = 0; i < count; ++i)
            {
                ParticleSystem.Particle particle = _particles[i];

                // get particle properties
                Vector2 position = (_particleSystem.simulationSpace == ParticleSystemSimulationSpace.Local ? particle.position : _transform.InverseTransformPoint(particle.position));
                float rotation = -particle.rotation * Mathf.Deg2Rad;
                float rotation90 = rotation + Mathf.PI / 2;
                Color32 color = particle.GetCurrentColor(_particleSystem);
                float size = particle.GetCurrentSize(_particleSystem) * 0.5f;

                // apply scale
                if (_particleSystem.scalingMode == ParticleSystemScalingMode.Shape)
                {
                    position /= canvas.scaleFactor;
                }

                // apply texture sheet animation
                Vector4 particleUV = _uv;
                if (_textureSheetAnimation.enabled)
                {
                    float frameProgress = 1 - (particle.remainingLifetime / particle.startLifetime);
                    //                float frameProgress = textureSheetAnimation.frameOverTime.curveMin.Evaluate(1 - (particle.lifetime / particle.startLifetime)); // TODO - once Unity allows MinMaxCurve reading
                    frameProgress = Mathf.Repeat(frameProgress * _textureSheetAnimation.cycleCount, 1);
                    int frame = 0;

                    switch (_textureSheetAnimation.animation)
                    {

                        case ParticleSystemAnimationType.WholeSheet:
                            frame = Mathf.FloorToInt(frameProgress * _textureSheetAnimationFrames);
                            break;

                        case ParticleSystemAnimationType.SingleRow:
                            frame = Mathf.FloorToInt(frameProgress * _textureSheetAnimation.numTilesX);

                            int row = _textureSheetAnimation.rowIndex;
                            //                    if (textureSheetAnimation.useRandomRow) { // FIXME - is this handled internally by rowIndex?
                            //                        row = Random.Range(0, textureSheetAnimation.numTilesY, using: particle.randomSeed);
                            //                    }
                            frame += row * _textureSheetAnimation.numTilesX;
                            break;

                    }

                    frame %= _textureSheetAnimationFrames;

                    particleUV.x = (frame % _textureSheetAnimation.numTilesX) * _textureSheedAnimationFrameSize.x;
                    particleUV.y = Mathf.FloorToInt(frame / _textureSheetAnimation.numTilesX) * _textureSheedAnimationFrameSize.y;
                    particleUV.z = particleUV.x + _textureSheedAnimationFrameSize.x;
                    particleUV.w = particleUV.y + _textureSheedAnimationFrameSize.y;
                }

                _quad[0] = UIVertex.simpleVert;
                _quad[0].color = color;
                _quad[0].uv0 = new Vector2(particleUV.x, particleUV.y);

                _quad[1] = UIVertex.simpleVert;
                _quad[1].color = color;
                _quad[1].uv0 = new Vector2(particleUV.x, particleUV.w);

                _quad[2] = UIVertex.simpleVert;
                _quad[2].color = color;
                _quad[2].uv0 = new Vector2(particleUV.z, particleUV.w);

                _quad[3] = UIVertex.simpleVert;
                _quad[3].color = color;
                _quad[3].uv0 = new Vector2(particleUV.z, particleUV.y);

                if (rotation == 0)
                {
                    // no rotation
                    Vector2 corner1 = new Vector2(position.x - size, position.y - size);
                    Vector2 corner2 = new Vector2(position.x + size, position.y + size);

                    _quad[0].position = new Vector2(corner1.x, corner1.y);
                    _quad[1].position = new Vector2(corner1.x, corner2.y);
                    _quad[2].position = new Vector2(corner2.x, corner2.y);
                    _quad[3].position = new Vector2(corner2.x, corner1.y);
                }
                else
                {
                    // apply rotation
                    Vector2 right = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)) * size;
                    Vector2 up = new Vector2(Mathf.Cos(rotation90), Mathf.Sin(rotation90)) * size;

                    _quad[0].position = position - right - up;
                    _quad[1].position = position - right + up;
                    _quad[2].position = position + right + up;
                    _quad[3].position = position + right - up;
                }

                vh.AddUIVertexQuad(_quad);
            }
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                // unscaled animation within UI
                _particleSystem.Simulate(Time.unscaledDeltaTime, false, false);

                SetAllDirty();
            }
        }

#if UNITY_EDITOR
        void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                SetAllDirty();
            }
        }
#endif
    }
#endif
}