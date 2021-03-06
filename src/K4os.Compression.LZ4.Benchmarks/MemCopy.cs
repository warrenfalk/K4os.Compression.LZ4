﻿using System;
using BenchmarkDotNet.Attributes;
using K4os.Compression.LZ4.Internal;

namespace K4os.Compression.LZ4.Benchmarks
{
	public unsafe class MemCopy
	{
		[Params(0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xA0)]
		public int Size { get; set; }

		private byte* _source;
		private byte* _target;

		[GlobalSetup]
		public void Setup()
		{
			_source = (byte*) Mem.Alloc(Size);
			_target = (byte*) Mem.Alloc(Size);
		}

		[GlobalCleanup]
		public void Cleanup()
		{
			Mem.Free(_source);
			Mem.Free(_target);
		}

		[Benchmark]
		public void WildCopy()
		{
			Mem.WildCopy(_target, _source, _target + Size);
		}
		
		[Benchmark]
		public void MemoryCopy()
		{
			var size = Size;
			Buffer.MemoryCopy(_source, _target, size, size);
		}
		
		[Benchmark]
		public void MemoryCopyAligned()
		{
			var size = ((Size - 1) & ~0x7) + 8;
			Buffer.MemoryCopy(_source, _target, size, size);
		}

		[Benchmark]
		public void HybridWildCopy()
		{
			var size = Size;
			if (size <= 128)
			{
				Mem.WildCopy(_target, _source, _target + size);
			}
			else
			{
				size = ((size - 1) & ~0x7) + 8;
				Buffer.MemoryCopy(_source, _target, size, size);
			}
		}
	}
}
