using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VD3V.SpriteSwapMorph
{
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("UI/Effects/Sprite Swap Morph Image")]
    [DisallowMultipleComponent]
    public class SpriteSwapMorphImage : SpriteSwapMorphBase, IMaterialModifier
    {
        private Image m_Image;
        private Sprite m_PrevSprite;
        private float m_MorphTime;
        private bool m_IsMorphing;
        private Material m_Material;


        protected virtual void Awake()
        {
            m_Image = GetComponent<Image>();
            RebuildMaterial();
        }

        protected virtual void OnEnable()
        {
            ResetState();
        }

        protected virtual void OnDestroy()
        {
            Destroy(m_Material);
        }

        protected override void UpdateMorph(float deltaTime)
        {
            if (m_Image.sprite != m_PrevSprite)
            {
                if (m_PrevSprite != null)
                    MaterialPropertyHelper.SetPrevSpriteTexture(m_Material, m_PrevSprite.texture);

                MaterialPropertyHelper.SetSpriteUVRect(m_Material, GetSpriteUVRect(m_Image.sprite));
                MaterialPropertyHelper.SetPrevSpriteUVRect(m_Material, GetSpriteUVRect(m_PrevSprite));
                MaterialPropertyHelper.SetMorphState(m_Material, true);

                m_PrevSprite = m_Image.sprite;

                if (m_IsMorphing)
                    m_MorphTime = 1 - m_MorphTime;
                else m_IsMorphing = true;

                m_Image.RecalculateMasking();
            }
            else if (m_IsMorphing)
            {
                m_MorphTime += deltaTime * MorphSpeed;

                if (m_MorphTime >= 1)
                {
                    ResetState();
                }
                else
                {
                    MaterialPropertyHelper.SetMorphTime(m_Material, m_MorphTime);
                    MaterialPropertyHelper.SetMorphBlurOverTime(m_Material, MorphBlurOverTime.Evaluate(m_MorphTime));

                    m_Image.RecalculateMasking();
                }
            }
        }

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            RebuildMaterial();
            ResetState();
        }

        protected virtual void RebuildMaterial()
        {
            if (m_Material != null)
                Destroy(m_Material);

            m_Material = OverrideMaterial != null ? new Material(OverrideMaterial) : new Material(Shader.Find("UI/SpriteSwapMorphImage"));
        }

        /// <summary>
        /// Resets morph state and material properties to their default values
        /// </summary>
        public virtual void ResetState()
        {
            if (m_Image != null)
            {
                m_PrevSprite = m_Image.sprite;
                m_Image.RecalculateMasking();
            }
            
            
            m_MorphTime = 0f;
            m_IsMorphing = false;
            MaterialPropertyHelper.SetMorphState(m_Material, false);
            MaterialPropertyHelper.SetMorphTime(m_Material, 0);
            MaterialPropertyHelper.SetMorphBlurOverTime(m_Material, 0);
            MaterialPropertyHelper.SetPrevSpriteTexture(m_Material, null);
        }

        /// <summary>
        /// Returns the modified material when morphing is active, otherwise returns the base material
        /// </summary>
        /// <param name="baseMaterial">The original material to modify</param>
        public virtual Material GetModifiedMaterial(Material baseMaterial)
        {
            return m_IsMorphing && m_Material != null ? m_Material : baseMaterial;
        }
    }
}
