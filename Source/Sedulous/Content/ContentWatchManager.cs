﻿using System;
using System.Collections.Generic;
using System.IO;
using Sedulous.Core;
using Sedulous.Platform;

namespace Sedulous.Content
{
    /// <summary>
    /// Contains methods for managing file watches for an instance of the <see cref="ContentManager"/> class.
    /// </summary>
    public class ContentWatchManager : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentWatchManager"/> class.
        /// </summary>
        /// <param name="context">The Framework context.</param>
        /// <param name="contentManager">The <see cref="ContentManager"/> instance that owns this file watcher.</param>
        internal ContentWatchManager(FrameworkContext context, ContentManager contentManager)
            : base(context)
        {
            Contract.Require(contentManager, nameof(contentManager));

            this.ContentManager = contentManager;
        }

        /// <summary>
        /// Adds a watcher for the specified asset.
        /// </summary>
        /// <param name="asset">The asset path of the asset for which to add a watcher.</param>
        /// <param name="watcher">The watcher to add for the specified asset.</param>
        /// <returns><see langword="true"/> if the watcher was added; otherwise, <see langword="false"/>.</returns>
        public Boolean AddWatcher<TOutput>(String asset, AssetWatcher<TOutput> watcher)
        {
            Contract.Require(asset, nameof(asset));
            Contract.Require(watcher, nameof(watcher));

            var primaryDisplay = ContentManager.FrameworkContext.GetPlatform().Displays.PrimaryDisplay;
            var primaryDisplayDensityBucket = primaryDisplay?.DensityBucket ?? ScreenDensityBucket.Desktop;

            return AddWatcherInternal(asset, primaryDisplayDensityBucket, watcher);
        }

        /// <summary>
        /// Adds a watcher for the specified asset.
        /// </summary>
        /// <param name="asset">The asset identifier of the asset for which to add a watcher.</param>
        /// <param name="watcher">The watcher to add for the specified asset.</param>
        /// <returns><see langword="true"/> if the watcher was added; otherwise, <see langword="false"/>.</returns>
        public Boolean AddWatcher<TOutput>(AssetId asset, AssetWatcher<TOutput> watcher)
        {
            Contract.Require(watcher, nameof(watcher));

            var primaryDisplay = ContentManager.FrameworkContext.GetPlatform().Displays.PrimaryDisplay;
            var primaryDisplayDensityBucket = primaryDisplay?.DensityBucket ?? ScreenDensityBucket.Desktop;

            return AddWatcherInternal(AssetId.GetAssetPath(asset), primaryDisplayDensityBucket, watcher);
        }

        /// <summary>
        /// Adds a watcher for the specified asset.
        /// </summary>
        /// <param name="asset">The asset path of the asset for which to add a watcher.</param>
        /// <param name="density">The density bucket corresponding to the version of the asset to watch.</param>
        /// <param name="watcher">The watcher to add for the specified asset.</param>
        /// <returns><see langword="true"/> if the watcher was added; otherwise, <see langword="false"/>.</returns>
        public Boolean AddWatcher<TOutput>(String asset, ScreenDensityBucket density, AssetWatcher<TOutput> watcher)
        {
            Contract.Require(asset, nameof(asset));
            Contract.Require(watcher, nameof(watcher));

            return AddWatcherInternal(asset, density, watcher);
        }

        /// <summary>
        /// Adds a watcher for the specified asset.
        /// </summary>
        /// <param name="asset">The asset identifier of the asset for which to add a watcher.</param>
        /// <param name="density">The density bucket corresponding to the version of the asset to watch.</param>
        /// <param name="watcher">The watcher to add for the specified asset.</param>
        /// <returns><see langword="true"/> if the watcher was added; otherwise, <see langword="false"/>.</returns>
        public Boolean AddWatcher<TOutput>(AssetId asset, ScreenDensityBucket density, AssetWatcher<TOutput> watcher)
        {
            Contract.Require(watcher, nameof(watcher));

            return AddWatcherInternal(AssetId.GetAssetPath(asset), density, watcher);
        }

        /// <summary>
        /// Removes a watcher from the specified asset.
        /// </summary>
        /// <param name="asset">The asset path of the asset for which to remove a watcher.</param>
        /// <param name="watcher">The watcher to remove from the specified asset.</param>
        /// <returns><see langword="true"/> if the specified watcher was removed; otherwise, <see langword="false"/>.</returns>
        public Boolean RemoveWatcher<TOutput>(String asset, AssetWatcher<TOutput> watcher)
        {
            Contract.Require(asset, nameof(asset));
            Contract.Require(watcher, nameof(watcher));

            var primaryDisplay = ContentManager.FrameworkContext.GetPlatform().Displays.PrimaryDisplay;
            var primaryDisplayDensityBucket = primaryDisplay?.DensityBucket ?? ScreenDensityBucket.Desktop;

            return RemoveWatcherInternal(asset, primaryDisplayDensityBucket, watcher);
        }

        /// <summary>
        /// Removes a watcher from the specified asset.
        /// </summary>
        /// <param name="asset">The asset identifier of the asset for which to remove a watcher.</param>
        /// <param name="watcher">The watcher to remove from the specified asset.</param>
        /// <returns><see langword="true"/> if the specified watcher was removed; otherwise, <see langword="false"/>.</returns>
        public Boolean RemoveWatcher<TOutput>(AssetId asset, AssetWatcher<TOutput> watcher)
        {
            Contract.Require(watcher, nameof(watcher));

            var primaryDisplay = ContentManager.FrameworkContext.GetPlatform().Displays.PrimaryDisplay;
            var primaryDisplayDensityBucket = primaryDisplay?.DensityBucket ?? ScreenDensityBucket.Desktop;

            return RemoveWatcherInternal(AssetId.GetAssetPath(asset), primaryDisplayDensityBucket, watcher);
        }

        /// <summary>
        /// Removes a watcher from the specified asset.
        /// </summary>
        /// <param name="asset">The asset path of the asset for which to remove a watcher.</param>
        /// <param name="density">The density bucket corresponding to the version of the asset to watch.</param>
        /// <param name="watcher">The watcher to remove from the specified asset.</param>
        /// <returns><see langword="true"/> if the specified watcher was removed; otherwise, <see langword="false"/>.</returns>
        public Boolean RemoveWatcher<TOutput>(String asset, ScreenDensityBucket density, AssetWatcher<TOutput> watcher)
        {
            Contract.Require(asset, nameof(asset));
            Contract.Require(watcher, nameof(watcher));

            return RemoveWatcherInternal(asset, density, watcher);
        }

        /// <summary>
        /// Removes a watcher from the specified asset.
        /// </summary>
        /// <param name="asset">The asset identifier of the asset for which to remove a watcher.</param>
        /// <param name="density">The density bucket corresponding to the version of the asset to watch.</param>
        /// <param name="watcher">The watcher to remove from the specified asset.</param>
        /// <returns><see langword="true"/> if the specified watcher was removed; otherwise, <see langword="false"/>.</returns>
        public Boolean RemoveWatcher<TOutput>(AssetId asset, ScreenDensityBucket density, AssetWatcher<TOutput> watcher)
        {
            Contract.Require(watcher, nameof(watcher));

            return RemoveWatcherInternal(AssetId.GetAssetPath(asset), density, watcher);
        }

        /// <summary>
        /// Gets a <see cref="WatchedAsset{T}"/> which watches the specified asset. The <see cref="WatchedAsset{T}"/> which is returned
        /// is owned by the content manager and shared between all callers of this method. If the watched asset has not already been
        /// loaded, it will be loaded and added to the content manager's internal cache.
        /// </summary>
        /// <typeparam name="TOutput">The type of object being loaded.</typeparam>
        /// <param name="asset">The path to the asset to load.</param>
        /// <returns>The <see cref="WatchedAsset{T}"/> instance which this content manager uses to watch the specified asset.</returns>
        public WatchedAsset<TOutput> GetSharedWatchedAsset<TOutput>(String asset)
        {
            Contract.RequireNotEmpty(asset, nameof(asset));
            Contract.EnsureNotDisposed(this, Disposed);

            var primaryDisplay = FrameworkContext.GetPlatform().Displays.PrimaryDisplay;
            var primaryDisplayDensity = primaryDisplay.DensityBucket;

            return GetSharedWatchedAssetInternal<TOutput>(asset, primaryDisplayDensity);
        }

        /// <summary>
        /// Gets a <see cref="WatchedAsset{T}"/> which watches the specified asset. The <see cref="WatchedAsset{T}"/> which is returned
        /// is owned by the content manager and shared between all callers of this method. If the watched asset has not already been
        /// loaded, it will be loaded and added to the content manager's internal cache.
        /// </summary>
        /// <typeparam name="TOutput">The type of object being loaded.</typeparam>
        /// <param name="asset">The identifier of the asset to load.</param>
        /// <returns>The <see cref="WatchedAsset{T}"/> instance which this content manager uses to watch the specified asset.</returns>
        public WatchedAsset<TOutput> GetSharedWatchedAsset<TOutput>(AssetId asset)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            var primaryDisplay = FrameworkContext.GetPlatform().Displays.PrimaryDisplay;
            var primaryDisplayDensity = primaryDisplay.DensityBucket;

            return GetSharedWatchedAssetInternal<TOutput>(AssetId.GetAssetPath(asset), primaryDisplayDensity);
        }

        /// <summary>
        /// Gets a <see cref="WatchedAsset{T}"/> which watches the specified asset. The <see cref="WatchedAsset{T}"/> which is returned
        /// is owned by the content manager and shared between all callers of this method. If the watched asset has not already been
        /// loaded, it will be loaded and added to the content manager's internal cache.
        /// </summary>
        /// <typeparam name="TOutput">The type of object being loaded.</typeparam>
        /// <param name="asset">The path to the asset to load.</param>
        /// <param name="density">The screen density for which to retrieve an asset watcher.</param>
        /// <returns>The <see cref="WatchedAsset{T}"/> instance which this content manager uses to watch the specified asset.</returns>
        public WatchedAsset<TOutput> GetSharedWatchedAsset<TOutput>(String asset, ScreenDensityBucket density)
        {
            Contract.RequireNotEmpty(asset, nameof(asset));
            Contract.EnsureNotDisposed(this, Disposed);

            return GetSharedWatchedAssetInternal<TOutput>(asset, density);
        }

        /// <summary>
        /// Gets a <see cref="WatchedAsset{T}"/> which watches the specified asset. The <see cref="WatchedAsset{T}"/> which is returned
        /// is owned by the content manager and shared between all callers of this method. If the watched asset has not already been
        /// loaded, it will be loaded and added to the content manager's internal cache.
        /// </summary>
        /// <typeparam name="TOutput">The type of object being loaded.</typeparam>
        /// <param name="asset">The identifier of the asset to load.</param>
        /// <param name="density">The screen density for which to retrieve an asset watcher.</param>
        /// <returns>The <see cref="WatchedAsset{T}"/> instance which this content manager uses to watch the specified asset.</returns>
        public WatchedAsset<TOutput> GetSharedWatchedAsset<TOutput>(AssetId asset, ScreenDensityBucket density)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return GetSharedWatchedAssetInternal<TOutput>(AssetId.GetAssetPath(asset), density);
        }

        /// <summary>
        /// Gets the <see cref="ContentManager"/> that owns this file watcher.
        /// </summary>
        public ContentManager ContentManager { get; }

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                lock (ContentManager.AssetCache.SyncObject)
                {
                    if (sharedWatchedAssets != null)
                    {
                        foreach (var sharedWatchedAsset in sharedWatchedAssets)
                        {
                            foreach (var kvp in sharedWatchedAsset.Value)
                                ((IDisposable)kvp.Value).Dispose();
                        }
                        sharedWatchedAssets.Clear();
                    }

                    rootFileSystemWatcher?.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the collection of asset watchers for the specified file.
        /// </summary>
        /// <param name="fullPath">The full path to the watched file.</param>
        /// <returns>An <see cref="IAssetWatcherCollection"/> that contains the file's asset watchers.</returns>
        internal IAssetWatcherCollection GetAssetWatchersForFile(String fullPath)
        {
            if (assetWatchers != null && assetWatchers.TryGetValue(fullPath, out var assetWatchersForFile))
                return assetWatchersForFile;

            return null;
        }

        /// <summary>
        /// Creates a set of file system watchers to monitor the content manager's file system for changes.
        /// </summary>
        private Boolean CreateFileSystemWatchers()
        {
            if (!ContentManager.IsWatchedContentSupported)
                return false;

            lock (ContentManager.AssetCache.SyncObject)
            {
                if (rootFileSystemWatcher == null)
                {
                    var rootdir = ContentDiscovery.FindSolutionDirectory(FrameworkContext, ContentManager.RootDirectory) ?? ContentManager.RootDirectory;
                    rootFileSystemWatcher = new FileSystemWatcher(rootdir);
                    rootFileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
                    rootFileSystemWatcher.IncludeSubdirectories = true;
                    rootFileSystemWatcher.EnableRaisingEvents = true;
                    rootFileSystemWatcher.Changed += ContentManager.OnFileSystemChanged;
                }

                ContentManager.OverrideDirectories.CreateFileSystemWatchers();
            }

            return true;
        }

        /// <summary>
        /// Adds a watcher to the specified asset.
        /// </summary>
        private Boolean AddWatcherInternal<TOutput>(String asset, ScreenDensityBucket density, AssetWatcher<TOutput> watcher)
        {
            lock (ContentManager.AssetCache.SyncObject)
            {
                if (CreateFileSystemWatchers())
                {
                    if (assetWatchers == null)
                        assetWatchers = new Dictionary<String, IAssetWatcherCollection>();

                    var assetPath = asset;
                    var assetResolvedPath = ContentManager.ResolveAssetFilePath(assetPath, density, true);
                    var assetFilePath = Path.GetFullPath(assetResolvedPath);

                    if (!assetWatchers.TryGetValue(assetFilePath, out var list))
                    {
                        list = new AssetWatcherCollection<TOutput>(ContentManager, assetPath, assetFilePath);
                        assetWatchers[assetFilePath] = list;
                    }

                    list.Add(watcher);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Removes a watcher from the specified asset.
        /// </summary>
        private Boolean RemoveWatcherInternal<TOutput>(String asset, ScreenDensityBucket density, AssetWatcher<TOutput> watcher)
        {
            lock (ContentManager.AssetCache.SyncObject)
            {
                if (assetWatchers == null)
                    return false;

                if (assetWatchers.TryGetValue(asset, out var list))
                    return list.Remove(watcher);

                return false;
            }
        }

        /// <summary>
        /// Gets a <see cref="WatchedAsset{T}"/> which watches the specified asset.
        /// </summary>
        private WatchedAsset<TOutput> GetSharedWatchedAssetInternal<TOutput>(String asset, ScreenDensityBucket density)
        {
            lock (ContentManager.AssetCache.SyncObject)
            {
                if (CreateFileSystemWatchers())
                {
                    if (sharedWatchedAssets == null)
                        sharedWatchedAssets = new Dictionary<String, Dictionary<Byte, Object>>();

                    if (!sharedWatchedAssets.TryGetValue(asset, out var watchersForAsset))
                    {
                        watchersForAsset = new Dictionary<Byte, Object>();
                        sharedWatchedAssets[asset] = watchersForAsset;
                    }

                    if (!watchersForAsset.TryGetValue((Byte)density, out var watcherForDensity))
                    {
                        watcherForDensity = new WatchedAsset<TOutput>(ContentManager, asset, density);
                        watchersForAsset[(Byte)density] = watcherForDensity;
                    }

                    return (WatchedAsset<TOutput>)watcherForDensity;
                }
            }

            return null;
        }

        // File watching.
        private FileSystemWatcher rootFileSystemWatcher;
        private Dictionary<String, IAssetWatcherCollection> assetWatchers;
        private Dictionary<String, Dictionary<Byte, Object>> sharedWatchedAssets;
    }
}
