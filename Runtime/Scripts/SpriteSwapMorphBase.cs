using UnityEngine;

namespace VD3V.SpriteSwapMorph
{
    public abstract class SpriteSwapMorphBase : MonoBehaviour
    {
        public static class MaterialPropertyHelper
        {
            public const string MorphStateOn = "_MORPHSTATE_ON";
            public const string MorphStateOff = "_MORPHSTATE_OFF";

            public static int MorphTimeId = Shader.PropertyToID("_MorphTime");
            public static int MorphBlurOverTimeId = Shader.PropertyToID("_MorphBlurOverTime");
            public static int SpriteUVRectId = Shader.PropertyToID("_SpriteUVRect");
            public static int PrevSpriteTextureId = Shader.PropertyToID("_PrevSpriteTexture");
            public static int PrevSpriteUVRectId = Shader.PropertyToID("_PrevSpriteUVRect");
            

            public static void SetMorphState(Material mat, bool enabled)
            {
                if (enabled)
                {
                    mat.EnableKeyword(MorphStateOn);
                    mat.DisableKeyword(MorphStateOff);
                }
                else
                {
                    mat.DisableKeyword(MorphStateOn);
                    mat.EnableKeyword(MorphStateOff);
                }
            }

            public static void SetMorphTime(Material mat, float morphTime) => mat.SetFloat(MorphTimeId, morphTime);

            public static void SetMorphBlurOverTime(Material mat, float morphBlurOverTime) => mat.SetFloat(MorphBlurOverTimeId, morphBlurOverTime);

            public static void SetPrevSpriteTexture(Material mat, Texture tex) => mat.SetTexture(PrevSpriteTextureId, tex);

            public static void SetSpriteUVRect(Material mat, Vector4 rect) => mat.SetVector(SpriteUVRectId, rect);

            public static void SetPrevSpriteUVRect(Material mat, Vector4 rect) => mat.SetVector(PrevSpriteUVRectId, rect);
        }

        public float MorphSpeed = 8f;
        public AnimationCurve MorphBlurOverTime = new(new Keyframe(0, 0), new Keyframe(0.5f, 20), new Keyframe(1, 0));
        public AnimatorUpdateMode UpdateMode = AnimatorUpdateMode.UnscaledTime;
        public Material OverrideMaterial;


        protected virtual void LateUpdate()
        {
            switch (UpdateMode)
            {
                case AnimatorUpdateMode.UnscaledTime:
                    UpdateMorph(Time.unscaledDeltaTime);
                    break;
                case AnimatorUpdateMode.Normal:
                    UpdateMorph(Time.deltaTime);
                    break;
            }
        }

        protected virtual void FixedUpdate()
        {
            switch (UpdateMode)
            {
#if UNITY_2023_1_OR_NEWER
                case AnimatorUpdateMode.Fixed:
#else
                case AnimatorUpdateMode.AnimatePhysics:
#endif
                    UpdateMorph(Time.fixedDeltaTime);
                    break;
            }
        }

        protected abstract void UpdateMorph(float deltaTime);

        protected virtual Vector4 GetSpriteUVRect(Sprite sprite)
        {
            if (sprite == null)
                return new Vector4(0, 0, 1, 1);

            var rect = new Vector4(sprite.rect.x, sprite.rect.y,
                sprite.rect.x + sprite.rect.width,
                sprite.rect.y + sprite.rect.height);
            rect.x /= sprite.texture.width;
            rect.y /= sprite.texture.height;
            rect.z /= sprite.texture.width;
            rect.w /= sprite.texture.height;

            return rect;
        }
    }
}

