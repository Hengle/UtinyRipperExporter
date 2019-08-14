﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uTinyRipper;
using uTinyRipper.AssetExporters;
using uTinyRipper.Classes;
using uTinyRipper.Classes.Shaders;
using uTinyRipper.Classes.Shaders.Exporters;
using Version = uTinyRipper.Version;

namespace Extract
{
	class DummyShaderTextExporter : ShaderTextExporter
	{
		static string DummyShader = @"
	//DummyShader
	SubShader{
		Tags { ""RenderType"" = ""Opaque"" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
		float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
";
		public override void Export(byte[] shaderData, TextWriter writer)
		{
			writer.Write("/*HelloWorld*/");
		}
		public static void ExportShader(Shader shader, IExportContainer container, Stream stream,
			Func<Version, GPUPlatform, ShaderTextExporter> exporterInstantiator)
		{
			//Importing Hidden/Internal shaders causes the unity editor screen to turn black
			if (shader.ParsedForm.Name.StartsWith("Hidden/")) return;
			if (Shader.IsSerialized(container.Version))
			{
				using (ShaderWriter writer = new ShaderWriter(stream, shader, exporterInstantiator))
				{
					writer.Write("Shader \"{0}\" {{\n", shader.ParsedForm.Name);
					shader.ParsedForm.PropInfo.Export(writer);
					writer.WriteIndent(1);
					writer.Write(DummyShader);
					if (shader.ParsedForm.FallbackName != string.Empty)
					{
						writer.WriteIndent(1);
						writer.Write("Fallback \"{0}\"\n", shader.ParsedForm.FallbackName);
					}
					if (shader.ParsedForm.CustomEditorName != string.Empty)
					{
						writer.WriteIndent(1);
						writer.Write("//CustomEditor \"{0}\"\n", shader.ParsedForm.CustomEditorName);
					}
					writer.Write('}');
				}
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}
