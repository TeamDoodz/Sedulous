﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Ultraviolet.Core;
using Ultraviolet.Core.Text;
using Ultraviolet.FreeType2.Native;
using Ultraviolet.Graphics;
using Ultraviolet.Graphics.Graphics2D;
using static Ultraviolet.FreeType2.Native.FreeTypeNative;
using static Ultraviolet.FreeType2.Native.FT_Error;
using static Ultraviolet.FreeType2.Native.FT_Kerning_Mode;
using static Ultraviolet.FreeType2.Native.FT_Pixel_Mode;
using static Ultraviolet.FreeType2.Native.FT_Render_Mode;
using static Ultraviolet.FreeType2.Native.FT_Stroker_LineCap;
using static Ultraviolet.FreeType2.Native.FT_Stroker_LineJoin;

namespace Ultraviolet.FreeType2
{
    /// <summary>
    /// Represents one of a FreeType font's font faces.
    /// </summary>
    public unsafe class FreeTypeFontFace : UltravioletFontFace
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreeTypeFontFace"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="face">The FreeType2 face which this instance represents.</param>
        /// <param name="metadata">The processor metadata with which this font face was loaded.</param>
        internal FreeTypeFontFace(UltravioletContext uv, IntPtr face, FreeTypeFontProcessorMetadata metadata)
            : base(uv)
        {
            Contract.Require(face, nameof(face));

            this.face = face;
            this.facade = new FT_FaceRecFacade(face);
            this.stroker = CreateStroker(face, metadata);

            if (metadata.AtlasWidth < 1 || metadata.AtlasHeight < 1 || metadata.AtlasSpacing < 0)
                throw new InvalidOperationException(FreeTypeStrings.InvalidAtlasParameters);

            this.AtlasWidth = metadata.AtlasWidth;
            this.AtlasHeight = metadata.AtlasHeight;
            this.AtlasSpacing = metadata.AtlasSpacing;

            this.offsetXAdjustment = metadata.AdjustOffsetX;
            this.offsetYAdjustment = metadata.AdjustOffsetY;
            this.ascenderAdjustment = metadata.AdjustAscender;
            this.descenderAdjustment = metadata.AdjustDescender;
            this.hAdvanceAdjustment = metadata.AdjustHorizontalAdvance;
            this.vAdvanceAdjustment = metadata.AdjustVerticalAdvance;
            this.glyphWidthAdjustment = (metadata.StrokeRadius * 2);
            this.glyphHeightAdjustment = (metadata.StrokeRadius * 2);

            this.HasColorStrikes = facade.HasColorFlag;
            this.HasKerningInfo = facade.HasKerningFlag;
            this.FamilyName = facade.MarshalFamilyName();
            this.StyleName = facade.MarshalStyleName();
            this.Ascender = facade.Ascender + ascenderAdjustment;
            this.Descender = facade.Descender + descenderAdjustment;
            this.LineSpacing = facade.LineSpacing;

            if (GetGlyphInfo(' ', out var spaceGlyphInfo))
                this.SpaceWidth = spaceGlyphInfo.Width;

            this.SizeInPoints = metadata.SizeInPoints;
            this.SizeInPixels = SizeInPixels;
            this.TabWidth = SpaceWidth * 4;

            this.totalDesignHeight = Ascender - Descender;

            this.SubstitutionCharacter = metadata.Substitution ??
                facade.GetCharIfDefined('�') ?? facade.GetCharIfDefined('□') ?? '?';

            PopulateGlyph(SubstitutionCharacter);
        }

        /// <inheritdoc/>
        public override String ToString() => String.Format("{0} {1} {2}pt", FamilyName, StyleName, SizeInPoints);

        /// <inheritdoc/>
        public override void GetGlyphRenderInfo(Int32 c, out GlyphRenderInfo info)
        {
            if (GetGlyphInfo((UInt32)c, out var ginfo))
            {
                info = new GlyphRenderInfo
                {
                    Texture = ginfo.Texture,
                    TextureRegion = ginfo.TextureRegion,
                    OffsetX = ginfo.OffsetX,
                    OffsetY = ginfo.OffsetY,
                    Advance = ginfo.Advance,
                };
            }
            else
            {
                info = default(GlyphRenderInfo);
            }
        }

        /// <inheritdoc/>
        public override Size2 MeasureString(String text)
        {
            var source = new StringSource(text);
            return MeasureString(ref source, 0, text.Length);
        }

        /// <inheritdoc/>
        public override Size2 MeasureString(String text, Int32 start, Int32 count)
        {
            var source = new StringSource(text);
            return MeasureString(ref source, start, count);
        }

        /// <inheritdoc/>
        public override Size2 MeasureString(StringBuilder text)
        {
            var source = new StringSource(text);
            return MeasureString(ref source, 0, text.Length);
        }

        /// <inheritdoc/>
        public override Size2 MeasureString(StringBuilder text, Int32 start, Int32 count)
        {
            var source = new StringSource(text);
            return MeasureString(ref source, start, count);
        }

        /// <inheritdoc/>
        public override Size2 MeasureString(ref StringSegment text)
        {
            var source = new StringSource(text);
            return MeasureString(ref source, 0, text.Length);
        }

        /// <inheritdoc/>
        public override Size2 MeasureString(ref StringSegment text, Int32 start, Int32 count)
        {
            var source = new StringSource(text);
            return MeasureString(ref source, start, count);
        }

        /// <inheritdoc/>
        public override Size2 MeasureString(ref StringSource source)
        {
            return MeasureString(ref source, 0, source.Length);
        }

        /// <inheritdoc/>
        public override Size2 MeasureString(ref StringSource source, Int32 start, Int32 count)
        {
            if (count == 0)
                return Size2.Zero;

            Contract.EnsureRange(start >= 0 && start < source.Length, nameof(start));
            Contract.EnsureRange(count >= 0 && start + count <= source.Length, nameof(count));

            var cx = 0;
            var cy = 0;
            for (var i = 0; i < count; i++)
            {
                var ix = start + i;
                var ixNext = ix + 1;

                var character = source[ix];
                if (ixNext < count && Char.IsSurrogatePair(source[ix], source[ixNext]))
                    i++;

                switch (character)
                {
                    case '\r':
                        continue;

                    case '\n':
                        cx = 0;
                        cy = cy + LineSpacing;
                        continue;

                    case '\t':
                        cx = cx + TabWidth;
                        continue;
                }

                cx += MeasureGlyph(ref source, ix).Width;
            }

            return new Size2(cx, cy + totalDesignHeight);
        }

        /// <inheritdoc/>
        public override Size2 MeasureGlyph(String text, Int32 ix)
        {
            var source = new StringSource(text);
            return MeasureGlyph(ref source, ix);
        }

        /// <inheritdoc/>
        public override Size2 MeasureGlyph(StringBuilder text, Int32 ix)
        {
            var source = new StringSource(text);
            return MeasureGlyph(ref source, ix);
        }

        /// <inheritdoc/>
        public override Size2 MeasureGlyph(ref StringSegment text, Int32 ix)
        {
            var source = new StringSource(text);
            return MeasureGlyph(ref source, ix);
        }

        /// <inheritdoc/>
        public override Size2 MeasureGlyphWithHypotheticalKerning(ref StringSegment text, Int32 ix, Int32 c2)
        {
            var source = new StringSource(text);
            return MeasureGlyphWithHypotheticalKerning(ref source, ix, c2);
        }

        /// <inheritdoc/>
        public override Size2 MeasureGlyphWithoutKerning(ref StringSegment text, Int32 ix)
        {
            var source = new StringSource(text);
            return MeasureGlyphWithoutKerning(ref source, ix);
        }

        /// <inheritdoc/>
        public override Size2 MeasureGlyph(ref StringSource text, Int32 ix)
        {
            GetUtf32CodePointFromString(ref text, ix, out var c1);
            if (!GetGlyphInfo(c1, out var cinfo))
                return Size2.Zero;

            var c2 = (ix + 1 < text.Length) ? text[ix + 1] : (Char?)null;
            var offset = c2.HasValue ? GetKerningInfo(text[ix], c2.GetValueOrDefault()) : Size2.Zero;            
            return new Size2(cinfo.Advance, totalDesignHeight) + offset;
        }

        /// <inheritdoc/>
        public override Size2 MeasureGlyphWithHypotheticalKerning(ref StringSource text, Int32 ix, Int32 c2)
        {
            GetUtf32CodePointFromString(ref text, ix, out var c1);
            if (!GetGlyphInfo(c1, out var cinfo))
                return Size2.Zero;

            var offset = GetKerningInfo(text[ix], c2);
            return new Size2(cinfo.Advance, totalDesignHeight) + offset;
        }

        /// <inheritdoc/>
        public override Size2 MeasureGlyphWithoutKerning(ref StringSource text, Int32 ix)
        {
            GetUtf32CodePointFromString(ref text, ix, out var c1);
            if (!GetGlyphInfo(c1, out var cinfo))
                return Size2.Zero;

            return new Size2(cinfo.Advance, totalDesignHeight);
        }

        /// <inheritdoc/>
        public override Size2 MeasureGlyph(Int32 c1, Int32? c2 = null)
        {
            if (!GetGlyphInfo((UInt32)c1, out var c1Info))
                return Size2.Zero;

            return new Size2(c1Info.Advance, totalDesignHeight);
        }

        /// <inheritdoc/>
        public override Size2 GetKerningInfo(Int32 c1, Int32 c2)
        {
            if (face == IntPtr.Zero || !HasKerningInfo)
                return Size2.Zero;

            var c1Index = facade.GetCharIndex((UInt32)c1);
            if (c1Index == 0)
                return Size2.Zero;

            var c2Index = facade.GetCharIndex((UInt32)c2);
            if (c2Index == 0)
                return Size2.Zero;

            if (Use64BitInterface)
            {
                var kerning = default(FT_Vector64);
                var err = FT_Get_Kerning(face, c1Index, c2Index, (UInt32)FT_KERNING_DEFAULT, (IntPtr)(&kerning));
                if (err != FT_Err_Ok)
                    throw new FreeTypeException(err);

                var x = FreeTypeCalc.F26Dot6ToInt32(kerning.x);
                var y = FreeTypeCalc.F26Dot6ToInt32(kerning.y);
                return new Size2(x, y);
            }
            else
            {
                var kerning = default(FT_Vector32);
                var err = FT_Get_Kerning(face, c1Index, c2Index, (UInt32)FT_KERNING_DEFAULT, (IntPtr)(&kerning));
                if (err != FT_Err_Ok)
                    throw new FreeTypeException(err);

                var x = FreeTypeCalc.F26Dot6ToInt32(kerning.x);
                var y = FreeTypeCalc.F26Dot6ToInt32(kerning.y);
                return new Size2(x, y);
            }
        }

        /// <inheritdoc/>
        public override Size2 GetKerningInfo(ref StringSource text, Int32 ix)
        {
            if (ix + 1 >= text.Length)
                return Size2.Zero;

            var pos = ix;
            pos += GetUtf32CodePointFromString(ref text, pos, out var c1) ? 2 : 1;
            if (pos >= text.Length)
                return Size2.Zero;

            GetUtf32CodePointFromString(ref text, pos, out var c2);

            return GetKerningInfo((Int32)c1, (Int32)c2);
        }

        /// <inheritdoc/>
        public override Size2 GetHypotheticalKerningInfo(ref StringSource text, Int32 ix, Int32 c2)
        {
            var c1 = text[ix];
            return GetKerningInfo(c1, c2);
        }

        /// <inheritdoc/>
        public override Size2 GetKerningInfo(ref StringSource text1, Int32 ix1, ref StringSource text2, Int32 ix2)
        {
            var c1 = text1[ix1];
            var c2 = text2[ix2];
            return GetKerningInfo(c1, c2);
        }

        /// <inheritdoc/>
        public override Size2 GetKerningInfo(ref StringSegment text, Int32 ix)
        {
            var source = new StringSource(text);
            return GetKerningInfo(ref source, ix);
        }

        /// <inheritdoc/>
        public override Size2 GetHypotheticalKerningInfo(ref StringSegment text, Int32 ix, Int32 c2)
        {
            var source = new StringSource(text);
            return GetHypotheticalKerningInfo(ref source, ix, c2);
        }

        /// <inheritdoc/>
        public override Size2 GetKerningInfo(ref StringSegment text1, Int32 ix1, ref StringSegment text2, Int32 ix2)
        {
            var source1 = new StringSource(text1);
            var source2 = new StringSource(text2);
            return GetKerningInfo(ref source1, ix1, ref source2, ix2);
        }

        /// <inheritdoc/>
        public override Boolean ContainsGlyph(Int32 c)
        {
            return facade.GetCharIndex((UInt32)c) > 0;
        }

        /// <summary>
        /// Ensures that the specified character's associated glyph has been added to the font's texture atlases.
        /// </summary>
        /// <param name="c">The character for which to ensure that a glyph has been populated.</param>
        public void PopulateGlyph(Char c)
        {
            GetGlyphInfo(c, out var info);
        }

        /// <summary>
        /// Gets the font's family name.
        /// </summary>
        public String FamilyName { get; }

        /// <summary>
        /// Gets the font's style name.
        /// </summary>
        public String StyleName { get; }

        /// <summary>
        /// Gets the width of the texture atlases used by FreeType2 font faces.
        /// </summary>
        public Int32 AtlasWidth { get; } = 1024;

        /// <summary>
        /// Gets the height of the texture atlases used by FreeType2 font faces.
        /// </summary>
        public Int32 AtlasHeight { get; } = 1024;

        /// <summary>
        /// Gets the spacing between cells on the atlases used by FreeType2 font faces.
        /// </summary>
        public Int32 AtlasSpacing { get; } = 4;

        /// <summary>
        /// Gets the font's size in points.
        /// </summary>
        public Int32 SizeInPoints { get; }

        /// <summary>
        /// Gets the font's size in pixels.
        /// </summary>
        public Int32 SizeInPixels { get; }

        /// <summary>
        /// Gets a value indicating whether this font has colored strikes.
        /// </summary>
        public Boolean HasColorStrikes { get; }

        /// <summary>
        /// Gets a value indicating whether this font has kerning information.
        /// </summary>
        public Boolean HasKerningInfo { get; }

        /// <inheritdoc/>
        public override Int32 Characters => glyphInfoCache.Count;

        /// <inheritdoc/>
        public override Int32 SpaceWidth { get; }

        /// <inheritdoc/>
        public override Int32 TabWidth { get; }

        /// <inheritdoc/>
        public override Int32 Ascender { get; }

        /// <inheritdoc/>
        public override Int32 Descender { get; }

        /// <inheritdoc/>
        public override Int32 LineSpacing { get; }

        /// <inheritdoc/>
        public override Char SubstitutionCharacter { get; }

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                if (stroker != IntPtr.Zero)
                {
                    FT_Stroker_Done(stroker);
                    stroker = IntPtr.Zero;
                }

                var err = FT_Done_Face(face);
                if (err != FT_Err_Ok)
                    throw new FreeTypeException(err);

                face = IntPtr.Zero;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates a stroker if this font face requires one.
        /// </summary>
        private static IntPtr CreateStroker(IntPtr face, FreeTypeFontProcessorMetadata metadata)
        {
            var err = default(FT_Error);
            var pstroker = IntPtr.Zero;

            if (metadata.StrokeRadius <= 0)
                return pstroker;

            err = FT_Stroker_New(FreeTypeFontPlugin.Library, (IntPtr)(&pstroker));
            if (err != FT_Err_Ok)
                throw new FreeTypeException(err);

            var lineCapMode = FT_STROKER_LINECAP_BUTT;
            switch (metadata.StrokeLineCap)
            {
                case FreeTypeLineCapMode.Round:
                    lineCapMode = FT_STROKER_LINECAP_ROUND;
                    break;

                case FreeTypeLineCapMode.Square:
                    lineCapMode = FT_STROKER_LINECAP_SQUARE;
                    break;
            }

            var lineJoinMode = FT_STROKER_LINEJOIN_ROUND;
            switch (metadata.StrokeLineJoin)
            {
                case FreeTypeLineJoinMode.Bevel:
                    lineJoinMode = FT_STROKER_LINEJOIN_BEVEL;
                    break;

                case FreeTypeLineJoinMode.Miter:
                    lineJoinMode = FT_STROKER_LINEJOIN_MITER;
                    break;

                case FreeTypeLineJoinMode.MiterFixed:
                    lineJoinMode = FT_STROKER_LINEJOIN_MITER_FIXED;
                    break;
            }

            var fixedRadius = FreeTypeCalc.Int32ToF26Dot6(metadata.StrokeRadius);
            var fixedMiterLimit = FreeTypeCalc.Int32ToF26Dot6(metadata.StrokeMiterLimit);

            if (Use64BitInterface)
                FT_Stroker_Set64(pstroker, fixedRadius, lineCapMode, lineJoinMode, fixedMiterLimit);
            else
                FT_Stroker_Set32(pstroker, fixedRadius, lineCapMode, lineJoinMode, fixedMiterLimit);

            return pstroker;
        }

        /// <summary>
        /// Performs gamma-corrected alpha blending between two colors.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GammaCorrectedAlphaBlend(ref Color src, ref Color dst, out Color result)
        {
            const Double Gamma = 2.2;
            const Double InverseGamma = 1.0 / Gamma;

            var srcA = src.A / 255.0;
            var srcR = Math.Pow((src.R / 255.0), Gamma);
            var srcG = Math.Pow((src.G / 255.0), Gamma);
            var srcB = Math.Pow((src.B / 255.0), Gamma);

            var dstA = dst.A / 255.0;
            var dstR = Math.Pow((dst.R / 255.0), Gamma);
            var dstG = Math.Pow((dst.G / 255.0), Gamma);
            var dstB = Math.Pow((dst.B / 255.0), Gamma);

            var invSrcA = (1f - srcA);
            var outR = (Single)Math.Pow(srcR + (dstR * invSrcA), InverseGamma);
            var outG = (Single)Math.Pow(srcG + (dstG * invSrcA), InverseGamma);
            var outB = (Single)Math.Pow(srcB + (dstB * invSrcA), InverseGamma);
            var outA = (Single)(srcA + (dstA * invSrcA));

            result = new Color(outR, outG, outB, 1f) * outA;
        }

        /// <summary>
        /// Blits the specified bitmap onto a texture atlas.
        /// </summary>
        private static void BlitBitmap(ref FT_Bitmap bmp, Int32 adjustX, Int32 adjustY, 
            Color color, FreeTypeBlendMode blendMode, ref DynamicTextureAtlas.Reservation reservation)
        {
            switch ((FT_Pixel_Mode)bmp.pixel_mode)
            {
                case FT_PIXEL_MODE_MONO:
                    BlitGlyphBitmapMono(ref bmp, adjustX, adjustY,
                        (Int32)bmp.width, (Int32)bmp.rows, bmp.pitch, color, blendMode, ref reservation);
                    break;

                case FT_PIXEL_MODE_GRAY:
                    BlitGlyphBitmapGray(ref bmp, adjustX, adjustY,
                        (Int32)bmp.width, (Int32)bmp.rows, bmp.pitch, color, blendMode, ref reservation);
                    break;

                case FT_PIXEL_MODE_BGRA:
                    BlitGlyphBitmapBgra(ref bmp, adjustX, adjustY,
                        (Int32)bmp.width, (Int32)bmp.rows, bmp.pitch, blendMode, ref reservation);
                    break;

                default:
                    throw new NotSupportedException(FreeTypeStrings.PixelFormatNotSupported);
            }
            reservation.Atlas.Invalidate();
        }

        /// <summary>
        /// Converts the specified index of a string to a UTF-32 codepoint.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Boolean GetUtf32CodePointFromString(ref StringSegment text, Int32 ix, out UInt32 utf32)
        {
            var ixNext = ix + 1;
            if (ixNext < text.Length)
            {
                var c1 = text[ix];
                var c2 = text[ixNext];
                if (Char.IsSurrogatePair(c1, c2))
                {
                    utf32 = (UInt32)Char.ConvertToUtf32(c1, c2);
                    return true;
                }
            }

            if (Char.IsSurrogate(text[ix]))
            {
                utf32 = SubstitutionCharacter;
                return false;
            }

            utf32 = text[ix];
            return false;
        }

        /// <summary>
        /// Converts the specified index of a string to a UTF-32 codepoint.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Boolean GetUtf32CodePointFromString(ref StringSource text, Int32 ix, out UInt32 utf32)
        {
            var ixNext = ix + 1;
            if (ixNext < text.Length)
            {
                var c1 = text[ix];
                var c2 = text[ixNext];
                if (Char.IsSurrogatePair(c1, c2))
                {
                    utf32 = (UInt32)Char.ConvertToUtf32(c1, c2);
                    return true;
                }
            }

            if (Char.IsSurrogate(text[ix]))
            {
                utf32 = SubstitutionCharacter;
                return false;
            }

            utf32 = text[ix];
            return false;
        }

        /// <summary>
        /// Blits a mono glyph bitmap to the specified atlas' surface.
        /// </summary>
        private static void BlitGlyphBitmapMono(ref FT_Bitmap bmp, Int32 adjustX, Int32 adjustY,
            Int32 bmpWidth, Int32 bmpHeight, Int32 bmpPitch, Color color, FreeTypeBlendMode blendMode, ref DynamicTextureAtlas.Reservation reservation)
        {
            var resX = reservation.X + adjustX;
            var resY = reservation.Y + adjustY;

            for (int y = 0; y < bmpHeight; y++)
            {
                var atlas = reservation.Atlas;
                var pSrcY = atlas.IsFlipped ? (bmpHeight - 1) - y : y;
                var pSrc = (Byte*)bmp.buffer + (pSrcY * bmpPitch);
                var pDst = (Color*)atlas.Surface.Pixels + ((resY + y) * atlas.Width) + resX;
                if (blendMode == FreeTypeBlendMode.Opaque)
                {
                    for (int x = 0; x < bmpWidth; x += 8)
                    {
                        var bits = *pSrc++;
                        for (int b = 0; b < 8; b++)
                        {
                            var srcColor = ((bits >> (7 - b)) & 1) == 0 ? Color.Transparent : color;
                            *pDst++ = srcColor;
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < bmpWidth; x += 8)
                    {
                        var bits = *pSrc++;
                        for (int b = 0; b < 8; b++)
                        {
                            var srcColor = ((bits >> (7 - b)) & 1) == 0 ? Color.Transparent : color;
                            var dstColor = *pDst;

                            GammaCorrectedAlphaBlend(ref srcColor, ref dstColor, out var result);
                            *pDst++ = result;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Blits a grayscale glyph bitmap to the specified atlas' surface.
        /// </summary>
        private static void BlitGlyphBitmapGray(ref FT_Bitmap bmp, Int32 adjustX, Int32 adjustY,
            Int32 bmpWidth, Int32 bmpHeight, Int32 bmpPitch, Color color, FreeTypeBlendMode blendMode, ref DynamicTextureAtlas.Reservation reservation)
        {
            var resX = reservation.X + adjustX;
            var resY = reservation.Y + adjustY;

            for (int y = 0; y < bmpHeight; y++)
            {
                var atlas = reservation.Atlas;
                var pSrcY = atlas.IsFlipped ? (bmpHeight - 1) - y : y;
                var pSrc = (Byte*)bmp.buffer + (pSrcY * bmpPitch);
                var pDst = (Color*)atlas.Surface.Pixels + ((resY + y) * atlas.Width) + resX;
                if (blendMode == FreeTypeBlendMode.Opaque)
                {
                    for (int x = 0; x < bmpWidth; x++)
                        *pDst++ = color * (*pSrc++ / 255f);
                }
                else
                {
                    for (int x = 0; x < bmpWidth; x++)
                    {
                        var srcColor = color * (*pSrc++ / 255f);
                        var dstColor = *pDst;

                        GammaCorrectedAlphaBlend(ref srcColor, ref dstColor, out var result);
                        *pDst++ = result;
                    }
                }
            }
        }

        /// <summary>
        /// Blits a BGRA color glyph bitmap to the specified atlas' surface.
        /// </summary>
        private static void BlitGlyphBitmapBgra(ref FT_Bitmap bmp, Int32 adjustX, Int32 adjustY,
            Int32 bmpWidth, Int32 bmpHeight, Int32 bmpPitch, FreeTypeBlendMode blendMode, ref DynamicTextureAtlas.Reservation reservation)
        {
            var resX = reservation.X + adjustX;
            var resY = reservation.Y + adjustY;

            for (int y = 0; y < bmpHeight; y++)
            {
                var atlas = reservation.Atlas;
                var pSrcY = atlas.IsFlipped ? (bmpHeight - 1) - y : y;
                var pSrc = (Color*)((Byte*)bmp.buffer + (pSrcY * bmpPitch));
                var pDst = (Color*)atlas.Surface.Pixels + ((resY + y) * atlas.Width) + resX;
                if (blendMode == FreeTypeBlendMode.Opaque)
                {
                    for (int x = 0; x < bmpWidth; x++)
                    {
                        var srcColor = *pSrc++;
                        var dstColor = new Color(srcColor.B, srcColor.G, srcColor.R, srcColor.A);
                        *pDst++ = dstColor;
                    }
                }
                else
                {
                    for (int x = 0; x < bmpWidth; x++)
                    {
                        var srcColor = *pSrc++;
                        var dstColor = *pDst;

                        GammaCorrectedAlphaBlend(ref srcColor, ref dstColor, out var result);
                        *pDst++ = result;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the font face's metadata for the specified glyph.
        /// </summary>
        private Boolean GetGlyphInfo(UInt32 value, out FreeTypeGlyphInfo info)
        {
            if (glyphInfoCache.TryGetValue(value, out var cached))
            {
                info = cached;
            }
            else
            {
                var cu16 = (value <= Char.MaxValue) ? (Char?)value : null;
                var index = facade.GetCharIndex(value);
                if (index == 0)
                {
                    if (cu16 != SubstitutionCharacter)
                        return GetGlyphInfo(SubstitutionCharacter, out info);

                    info = default(FreeTypeGlyphInfo);
                    return false;
                }
                else
                {
                    LoadGlyphTexture(cu16, value, index, out info);
                    glyphInfoCache[value] = info;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Loads the texture data for the specified glyph.
        /// </summary>
        private void LoadGlyphTexture(Char? cu16, UInt32 cu32, UInt32 glyphIndex, out FreeTypeGlyphInfo info)
        {
            var err = default(FT_Error);

            // Load the glyph into the face's glyph slot.
            var flags = (stroker == IntPtr.Zero) ? FT_LOAD_COLOR : FT_LOAD_NO_BITMAP;
            err = FT_Load_Glyph(face, glyphIndex, flags);
            if (err != FT_Err_Ok)
                throw new FreeTypeException(err);

            var glyphOffsetX = 0;
            var glyphOffsetY = 0;
            var glyphAscent = 0;
            var glyphAdvance = (cu16 == '\t') ? 
                (facade.GlyphMetricHorizontalAdvance + hAdvanceAdjustment) * 4 : 
                (facade.GlyphMetricHorizontalAdvance + hAdvanceAdjustment);

            // If the glyph is not whitespace, we need to add it to one of our atlases.
            var reservation = default(DynamicTextureAtlas.Reservation);
            var reservationFound = false;
            if (!cu16.HasValue || !Char.IsWhiteSpace(cu16.GetValueOrDefault()))
            {
                // Stroke the glyph.   
                StrokeGlyph(out var strokeGlyph, out var strokeBmp, 
                    out var strokeOffsetX, out var strokeOffsetY);
                
                // Render the glyph.
                err = FT_Render_Glyph(facade.GlyphSlot, FT_RENDER_MODE_NORMAL);
                if (err != FT_Err_Ok)
                    throw new FreeTypeException(err);

                var glyphBmp = facade.GlyphBitmap;
                var glyphAdjustX = 0;
                var glyphAdjustY = 0;

                var reservationWidth = 0;
                var reservationHeight = 0;
                if (strokeGlyph == IntPtr.Zero)
                {
                    reservationWidth = (Int32)glyphBmp.width;
                    reservationHeight = (Int32)glyphBmp.rows;

                    glyphAscent = facade.GlyphBitmapTop;
                }
                else
                {
                    reservationWidth = Math.Max((Int32)strokeBmp.width, (Int32)glyphBmp.width);
                    reservationHeight = Math.Max((Int32)strokeBmp.rows, (Int32)glyphBmp.rows);

                    glyphAscent = Math.Max(facade.GlyphBitmapTop, strokeOffsetY);
                    glyphAdjustX = -(strokeOffsetX - facade.GlyphBitmapLeft);
                    glyphAdjustY = strokeOffsetY - facade.GlyphBitmapTop;
                }

                // Attempt to reserve space on one of the font's existing atlases.
                foreach (var atlas in atlases)
                {
                    if (atlas.TryReserveCell(reservationWidth, reservationHeight, out reservation))
                    {
                        reservationFound = true;
                        break;
                    }
                }

                // Attempt to create a new atlas if we weren't able to make a reservation.
                if (!reservationFound)
                {
                    var atlas = DynamicTextureAtlas.Create(AtlasWidth, AtlasHeight, AtlasSpacing);
                    atlas.Surface.Clear(Color.Transparent);
                    atlases.Add(atlas);

                    if (!atlas.TryReserveCell(reservationWidth, reservationHeight, out reservation))
                        throw new InvalidOperationException(FreeTypeStrings.GlyphTooBigForAtlas.Format(cu16));
                }

                // Update the atlas surface.
                var blendMode = FreeTypeBlendMode.Opaque;
                if (strokeGlyph != IntPtr.Zero)
                {
                    BlitBitmap(ref strokeBmp, 0, 0, Color.Black, blendMode, ref reservation);
                    blendMode = FreeTypeBlendMode.Blend;

                    FT_Done_Glyph(strokeGlyph);
                }
                BlitBitmap(ref glyphBmp, glyphAdjustX, glyphAdjustY, Color.White, blendMode, ref reservation);
            }

            // Calculate the glyph's metrics.
            glyphOffsetX = facade.GlyphBitmapLeft + offsetXAdjustment;
            glyphOffsetY = (Ascender - glyphAscent) + offsetYAdjustment;

            info = new FreeTypeGlyphInfo
            {
                UnicodeCharacter = cu32,
                Advance = glyphAdvance,
                Width = facade.GlyphMetricWidth + glyphWidthAdjustment,
                Height = facade.GlyphMetricHeight + glyphHeightAdjustment,
                OffsetX = glyphOffsetX,
                OffsetY = glyphOffsetY,
                Texture = reservation.Atlas,
                TextureRegion = reservation.Atlas == null ? Rectangle.Empty : reservation.Atlas.IsFlipped ?
                    new Rectangle(reservation.X, reservation.Atlas.Height - (reservation.Y + reservation.Height), reservation.Width, reservation.Height) :
                    new Rectangle(reservation.X, reservation.Y, reservation.Width, reservation.Height),
            };
        }

        /// <summary>
        /// Strokes the currently loaded glyph, if a stroker exists.
        /// </summary>
        private void StrokeGlyph(out IntPtr glyph, out FT_Bitmap strokeBmp, out Int32 strokeOffsetX, out Int32 strokeOffsetY)
        {
            var err = default(FT_Error);
            var strokeGlyph = IntPtr.Zero;

            if (stroker == IntPtr.Zero)
            {
                glyph = IntPtr.Zero;
                strokeBmp = default(FT_Bitmap);
                strokeOffsetX = 0;
                strokeOffsetY = 0;
                return;
            }

            err = FT_Get_Glyph(facade.GlyphSlot, (IntPtr)(&strokeGlyph));
            if (err != FT_Err_Ok)
                throw new FreeTypeException(err);

            err = FT_Glyph_StrokeBorder((IntPtr)(&strokeGlyph), stroker, false, true);
            if (err != FT_Err_Ok)
            {
                FT_Done_Glyph(strokeGlyph);
                throw new FreeTypeException(err);
            }

            err = FT_Glyph_To_Bitmap((IntPtr)(&strokeGlyph), FT_RENDER_MODE_NORMAL, IntPtr.Zero, true);
            if (err != FT_Err_Ok)
            {
                FT_Done_Glyph(strokeGlyph);
                throw new FreeTypeException(err);
            }

            if (Use64BitInterface)
            {
                var bmp64 = (FT_BitmapGlyphRec64*)strokeGlyph;
                strokeBmp = bmp64->bitmap;
                strokeOffsetX = bmp64->left;
                strokeOffsetY = bmp64->top;
            }
            else
            {
                var bmp32 = (FT_BitmapGlyphRec32*)strokeGlyph;
                strokeBmp = bmp32->bitmap;
                strokeOffsetX = bmp32->left;
                strokeOffsetY = bmp32->top;
            }

            glyph = strokeGlyph;
        }

        // The FreeType2 face which this instance represents.
        private readonly FT_FaceRecFacade facade;
        private IntPtr face;
        private IntPtr stroker;

        // Metric adjustments.
        private readonly Int32 totalDesignHeight;
        private readonly Int32 offsetXAdjustment;
        private readonly Int32 offsetYAdjustment;
        private readonly Int32 ascenderAdjustment;
        private readonly Int32 descenderAdjustment;
        private readonly Int32 hAdvanceAdjustment;
        private readonly Int32 vAdvanceAdjustment;
        private readonly Int32 glyphWidthAdjustment;
        private readonly Int32 glyphHeightAdjustment;

        // Cache of atlases used to store glyph images.
        private readonly List<DynamicTextureAtlas> atlases = 
            new List<DynamicTextureAtlas>();

        // Cache of metadata for loaded glyphs.
        private readonly Dictionary<UInt32, FreeTypeGlyphInfo> glyphInfoCache =
            new Dictionary<UInt32, FreeTypeGlyphInfo>();
    }
}
