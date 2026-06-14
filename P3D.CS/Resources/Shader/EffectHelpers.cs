using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

[Flags]
public enum EffectDirtyFlags
{
    WorldViewProj = 1,
    World = 2,
    EyePosition = 4,
    MaterialColor = 8,
    Fog = 16,
    FogEnable = 32,
    AlphaTest = 64,
    ShaderIndex = 128,
    All = -1,
}

public static class EffectHelpers
{
    public static Vector3 EnableDefaultLighting(DirectionalLight light0, DirectionalLight light1, DirectionalLight light2)
    {
        light0.Direction = new Vector3(-0.5265408f, -0.5735765f, -0.6275069f);
        light0.DiffuseColor = new Vector3(1, 0.9607844f, 0.8078432f);
        light0.SpecularColor = new Vector3(1, 0.9607844f, 0.8078432f);
        light0.Enabled = true;

        light1.Direction = new Vector3(0.7198464f, 0.3420201f, 0.6040227f);
        light1.DiffuseColor = new Vector3(0.9647059f, 0.7607844f, 0.4078432f);
        light1.SpecularColor = Vector3.Zero;
        light1.Enabled = true;

        light2.Direction = new Vector3(0.4545195f, -0.7660444f, 0.4545195f);
        light2.DiffuseColor = new Vector3(0.3231373f, 0.3607844f, 0.3937255f);
        light2.SpecularColor = new Vector3(0.3231373f, 0.3607844f, 0.3937255f);
        light2.Enabled = true;

        return new Vector3(0.05333332f, 0.09882354f, 0.1819608f);
    }

    public static EffectDirtyFlags SetWorldViewProjAndFog(EffectDirtyFlags dirtyFlags,
        ref Matrix world, ref Matrix view, ref Matrix projection, ref Matrix worldView,
        bool fogEnabled, float fogStart, float fogEnd,
        EffectParameter worldViewProjParam, EffectParameter fogVectorParam)
    {
        if ((dirtyFlags & EffectDirtyFlags.WorldViewProj) != 0)
        {
            worldView = world * view;
            worldViewProjParam.SetValue(worldView * projection);
            dirtyFlags &= ~EffectDirtyFlags.WorldViewProj;
        }

        if (fogEnabled == true)
        {
            if ((dirtyFlags & (EffectDirtyFlags.Fog | EffectDirtyFlags.FogEnable)) != 0)
            {
                SetFogVector(ref worldView, fogStart, fogEnd, fogVectorParam);
                dirtyFlags &= ~(EffectDirtyFlags.Fog | EffectDirtyFlags.FogEnable);
            }
        }
        else
        {
            if ((dirtyFlags & EffectDirtyFlags.FogEnable) != 0)
            {
                fogVectorParam.SetValue(Vector4.Zero);
                dirtyFlags &= ~EffectDirtyFlags.FogEnable;
            }
        }

        return dirtyFlags;
    }

    public static EffectDirtyFlags SetLightingMatrices(EffectDirtyFlags dirtyFlags,
        ref Matrix world, ref Matrix view,
        EffectParameter worldParam, EffectParameter worldInverseTransposeParam, EffectParameter eyePositionParam)
    {
        if ((dirtyFlags & EffectDirtyFlags.World) != 0)
        {
            worldParam.SetValue(world);
            worldInverseTransposeParam.SetValue(Matrix.Transpose(Matrix.Invert(world)));
            dirtyFlags &= ~EffectDirtyFlags.World;
        }

        if ((dirtyFlags & EffectDirtyFlags.EyePosition) != 0)
        {
            eyePositionParam.SetValue(Matrix.Invert(view).Translation);
            dirtyFlags &= ~EffectDirtyFlags.EyePosition;
        }

        return dirtyFlags;
    }

    public static void SetMaterialColor(bool lightingEnabled, float alpha,
        ref Vector3 diffuseColor, ref Vector3 emissiveColor, ref Vector3 ambientLightColor,
        EffectParameter diffuseColorParam, EffectParameter emissiveColorParam)
    {
        if (lightingEnabled == true)
        {
            diffuseColorParam.SetValue(new Vector4(diffuseColor.X * alpha, diffuseColor.Y * alpha, diffuseColor.Z * alpha, alpha));
            emissiveColorParam.SetValue(new Vector3(
                (emissiveColor.X + ambientLightColor.X * diffuseColor.X) * alpha,
                (emissiveColor.Y + ambientLightColor.Y * diffuseColor.Y) * alpha,
                (emissiveColor.Z + ambientLightColor.Z * diffuseColor.Z) * alpha));
        }
        else
        {
            diffuseColorParam.SetValue(new Vector4(
                (diffuseColor.X + emissiveColor.X) * alpha,
                (diffuseColor.Y + emissiveColor.Y) * alpha,
                (diffuseColor.Z + emissiveColor.Z) * alpha,
                alpha));
        }
    }

    private static void SetFogVector(ref Matrix worldView, float fogStart, float fogEnd, EffectParameter fogVectorParam)
    {
        if (fogStart == fogEnd)
        {
            fogVectorParam.SetValue(new Vector4(0, 0, 0, 1));
        }
        else
        {
            float scale = 1f / (fogStart - fogEnd);
            fogVectorParam.SetValue(new Vector4(
                worldView.M13 * scale,
                worldView.M23 * scale,
                worldView.M33 * scale,
                (worldView.M43 + fogStart) * scale));
        }
    }
}
