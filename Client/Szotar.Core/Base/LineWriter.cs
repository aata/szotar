using System.IO;
using System.Text;
using System;

namespace Szotar {
	public class LineWriter : IDisposable {
		Encoder encoder;

		byte[] writeBuffer;
		int writeBufferOffset;

		Stream stream;
		long streamPosition;

		public LineWriter(Stream stream)
			: this(stream, 4096)
		{ }

		void Close() {
			FlushBuffer();
			stream.Close();
		}

		public LineWriter(Stream stream, int bufferSize) {
			if (bufferSize < 1024)
				bufferSize = 1024;

			this.stream = stream;
			writeBuffer = new byte[bufferSize];
			writeBufferOffset = 0;
			encoder = Encoding.UTF8.GetEncoder();
		}

		public long Position {
			get {
				return streamPosition;
			}
		}

		void Write(string str, bool terminateLine) {
			char[] chars = str.ToCharArray();
			int index = 0;
			int totalBytes = 0;

			while (index < chars.Length) {
				int charsUsed, bytesUsed;
				bool completed;

				// This is slightly strange.
				// The Encoder complains if there isn't room in the buffer for at least one character.
				// I tested WriteLine(new string('x', 10000)) with a buffer size of 1024 and it worked fine,
				// so there shouldn't be any more exceptions being thrown.
				// The maximum bytes per code point for UTF-8 is 4.
				if (writeBufferOffset + 4 >= writeBuffer.Length)
					FlushBuffer();

				encoder.Convert(
					chars,
					index,
					str.Length - index,
					writeBuffer,
					writeBufferOffset,
					writeBuffer.Length - writeBufferOffset,
					false,
					out charsUsed,
					out bytesUsed,
					out completed);

				index += charsUsed;
				totalBytes += bytesUsed;
				writeBufferOffset += bytesUsed;

				// If it wasn't completed, it's because the buffer wasn't big enough.
				// TODO: asynchronous I/O.
				if (!completed)
					FlushBuffer();
			}

			streamPosition += totalBytes;

			if (terminateLine) {
				// Now, write the line terminator.
				WriteByte((byte)'\r');
				WriteByte((byte)'\n');
			}
			
			System.Diagnostics.Debug.Assert(streamPosition >= stream.Position);
		}

		public void Write(string str) {
			Write(str, false);
		}

		public void WriteLine(string line) {
			Write(line, true);
		}

		void WriteByte(byte b) {
			if (writeBufferOffset >= writeBuffer.Length)
				FlushBuffer();

			writeBuffer[writeBufferOffset++] = b;
			streamPosition++;
		}

		void FlushBuffer() {
			if (writeBufferOffset > 0)
				stream.Write(writeBuffer, 0, writeBufferOffset);

			writeBufferOffset = 0;
		}

		#region Dispose
		public void Dispose() {
			Dispose(true);
		}

		protected void Dispose(bool disposing) {
			if (disposing) {
				Close();
				stream.Dispose();
			}

			GC.SuppressFinalize(this);
		}
		#endregion
	}
}