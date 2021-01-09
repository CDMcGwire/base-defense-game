using System;
using System.IO;
using System.Threading.Tasks;

namespace data.service {
/// <summary>
/// An interface that denotes the service can read and write information from a
/// section of a save file.
/// </summary>
public interface IPersistableService {
	Task WriteLines(StreamWriter stream);
	Task ReadLines(SectionReader stream);
}

/// <summary>
/// A wrapper for a StreamReader which features an auto-shutoff when the given
/// 'finalLine' value is read. A reset method is supplied to allow for multiple
/// uses on the same stream without reallocating resources.
/// </summary>
public class SectionReader {
	private readonly StreamReader stream;
	private readonly string finalLine;
	private bool foundEnd;

	public SectionReader(StreamReader stream, string finalLine) {
		this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
		this.finalLine = finalLine ?? throw new ArgumentNullException(nameof(finalLine));
	}

	public string ReadLine()
		=> foundEnd ? null : Process(stream.ReadLine());

	public async Task<string> ReadLineAsync()
		=> foundEnd ? null : Process(await stream.ReadLineAsync());

	public void Reset() => foundEnd = false;
	
	private string Process(string line) {
		if (line != null && line != finalLine)
			return line;
		foundEnd = true;
		return null;
	}
}
}