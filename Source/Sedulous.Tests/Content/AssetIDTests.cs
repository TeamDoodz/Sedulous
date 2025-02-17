﻿using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using Sedulous.Content;
using Sedulous.TestApplication;

namespace Sedulous.Tests.Content
{
    [TestFixture]
    public class AssetIDTests : FrameworkApplicationTestFramework
    {
        [Test]
        [Category("Content")]
        public void AssetID_SerializesToJson()
        {
            GivenAFrameworkApplicationWithNoWindow()
                .WithContent(content =>
                {
                    content.FrameworkContext.GetContent().Manifests.Load(Path.Combine("Resources", "Content", "Manifests", "Test.manifest"));
                    
                    var id = content.FrameworkContext.GetContent().Manifests["Test"]["Textures"]["Triangle"].CreateAssetId();
                    var json = JsonConvert.SerializeObject(id, 
                        FrameworkJsonSerializerSettings.Instance);

                    TheResultingString(json)
                        .ShouldBe(@"""#Test:Textures:Triangle""");
                })
                .RunForOneFrame();
        }

        [Test]
        [Category("Content")]
        public void AssetID_DeserializesFromJson_WithXmlManifest()
        {
            GivenAFrameworkApplicationWithNoWindow()
                .WithContent(content =>
                {
                    content.FrameworkContext.GetContent().Manifests.Load(Path.Combine("Resources", "Content", "Manifests", "Test.manifest"));
                    
                    var id = JsonConvert.DeserializeObject<AssetId>(@"""#Test:Textures:Triangle""", 
                        FrameworkJsonSerializerSettings.Instance);

                    TheResultingValue(id)
                        .ShouldBe(AssetId.Parse("#Test:Textures:Triangle"));
                })
                .RunForOneFrame();
        }

        [Test]
        [Category("Content")]
        public void AssetID_DeserializesFromJson_WithJsonManifest()
        {
            GivenAFrameworkApplicationWithNoWindow()
                .WithContent(content =>
                {
                    content.FrameworkContext.GetContent().Manifests.Load(Path.Combine("Resources", "Content", "Manifests", "TestJson.jsmanifest"));

                    var id = JsonConvert.DeserializeObject<AssetId>(@"""#Test:Textures:Triangle""",
                        FrameworkJsonSerializerSettings.Instance);

                    TheResultingValue(id)
                        .ShouldBe(AssetId.Parse("#Test:Textures:Triangle"));
                })
                .RunForOneFrame();
        }
    }
}
