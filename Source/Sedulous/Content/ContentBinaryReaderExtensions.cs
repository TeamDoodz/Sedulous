﻿using System.IO;
using Sedulous.Core;

namespace Sedulous.Content
{
    /// <summary>
    /// Contains extension methods for the <see cref="System.IO.BinaryReader"/> class.
    /// </summary>
    public static class ContentBinaryReaderExtensions
    {
        /// <summary>
        /// Reads an asset identifier from the stream using the content manifest registry
        /// belonging to the current Framework context.
        /// </summary>
        /// <param name="reader">The binary reader from which to read the asset identifier.</param>
        /// <returns>An instance of the <see cref="AssetId"/> structure representing the 
        /// asset identifier that was read from the stream.</returns>
        public static AssetId ReadAssetId(this BinaryReader reader)
        {
            Contract.Require(reader, nameof(reader));

            return ReadAssetId(reader, FrameworkContext.DemandCurrent().GetContent().Manifests);
        }

        /// <summary>
        /// Reads a nullable asset identifier from the stream using the content manifest registry
        /// belonging to the current Framework context.
        /// </summary>
        /// <param name="reader">The binary reader from which to read the asset identifier.</param>
        /// <returns>An instance of the <see cref="System.Nullable{AssetID}"/> structure representing the 
        /// asset identifier that was read from the stream.</returns>
        public static AssetId? ReadNullableAssetId(this BinaryReader reader)
        {
            Contract.Require(reader, nameof(reader));

            return ReadNullableAssetId(reader, FrameworkContext.DemandCurrent().GetContent().Manifests);
        }

        /// <summary>
        /// Reads an asset identifier from the stream.
        /// </summary>
        /// <param name="reader">The binary reader from which to read the asset identifier.</param>
        /// <param name="manifests">The registry that contains the application's loaded manifests.</param>
        /// <returns>An instance of the <see cref="AssetId"/> structure representing the
        /// asset identifier that was read from the stream.</returns>
        public static AssetId ReadAssetId(this BinaryReader reader, ContentManifestRegistry manifests)
        {
            Contract.Require(reader, nameof(reader));
            Contract.Require(manifests, nameof(manifests));

            var valid = reader.ReadBoolean();
            if (valid)
            {
                var manifestName = reader.ReadString();
                var manifestGroupName = reader.ReadString();
                var assetName = reader.ReadString();

                var manifest = manifests[manifestName];
                if (manifest == null)
                    return AssetId.Invalid;

                var manifestGroup = manifest[manifestGroupName];
                if (manifestGroup == null)
                    return AssetId.Invalid;

                var asset = manifestGroup[assetName];
                if (asset == null)
                    return AssetId.Invalid;

                return asset.CreateAssetId();
            }
            return AssetId.Invalid;
        }

        /// <summary>
        /// Reads a nullable asset identifier from the stream.
        /// </summary>
        /// <param name="reader">The binary reader from which to read the asset identifier.</param>
        /// <param name="manifests">The registry that contains the application's loaded manifests.</param>
        /// <returns>An instance of the <see cref="System.Nullable{AssetID}"/> structure representing the
        /// asset identifier that was read from the stream.</returns>
        public static AssetId? ReadNullableAssetId(this BinaryReader reader, ContentManifestRegistry manifests)
        {
            Contract.Require(reader, nameof(reader));
            Contract.Require(manifests, nameof(manifests));

            var hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                return reader.ReadAssetId(manifests);
            }
            return null;
        }
    }
}
