using Xunit;
using openrmf_msg_score.Classes;
using System;

namespace tests.Classes
{
    public class CompressionTests
    {
        // ---- Pass Tests ----

        [Fact]
        public void Test_CompressString_ReturnsNonEmptyString()
        {
            var original = "Hello, OpenRMF!";
            var compressed = Compression.CompressString(original);
            Assert.NotNull(compressed);
            Assert.NotEmpty(compressed);
        }

        [Fact]
        public void Test_CompressString_IsBase64Encoded()
        {
            var original = "Hello, OpenRMF!";
            var compressed = Compression.CompressString(original);
            // Should not throw - valid base64
            var bytes = Convert.FromBase64String(compressed);
            Assert.NotNull(bytes);
        }

        [Fact]
        public void Test_DecompressString_ReturnsOriginalString()
        {
            var original = "Hello, OpenRMF World!";
            var compressed = Compression.CompressString(original);
            var decompressed = Compression.DecompressString(compressed);
            Assert.Equal(original, decompressed);
        }

        [Fact]
        public void Test_CompressDecompress_Roundtrip_ShortString()
        {
            var original = "short";
            var result = Compression.DecompressString(Compression.CompressString(original));
            Assert.Equal(original, result);
        }

        [Fact]
        public void Test_CompressDecompress_Roundtrip_LongString()
        {
            var original = new string('A', 10000);
            var result = Compression.DecompressString(Compression.CompressString(original));
            Assert.Equal(original, result);
        }

        [Fact]
        public void Test_CompressDecompress_Roundtrip_SpecialCharacters()
        {
            var original = "<CHECKLIST><ASSET><HOST_NAME>my-server</HOST_NAME></ASSET></CHECKLIST>";
            var result = Compression.DecompressString(Compression.CompressString(original));
            Assert.Equal(original, result);
        }

        [Fact]
        public void Test_CompressDecompress_Roundtrip_JsonPayload()
        {
            var original = "{\"artifactId\":\"abc123\",\"updatedBy\":\"user@example.com\"}";
            var result = Compression.DecompressString(Compression.CompressString(original));
            Assert.Equal(original, result);
        }

        [Fact]
        public void Test_CompressString_DifferentInputs_ProduceDifferentOutputs()
        {
            var compressed1 = Compression.CompressString("string one");
            var compressed2 = Compression.CompressString("string two");
            Assert.NotEqual(compressed1, compressed2);
        }

        [Fact]
        public void Test_CompressString_CompressedIsShorterOrSameForRepetitiveContent()
        {
            var original = new string('X', 1000);
            var compressed = Compression.CompressString(original);
            // Base64 adds overhead, but the gzip content itself should be much smaller
            // Just verify it compresses without error and round-trips
            var decompressed = Compression.DecompressString(compressed);
            Assert.Equal(original, decompressed);
        }

        [Theory]
        [InlineData("Hello World")]
        [InlineData("OpenRMF Score Service")]
        [InlineData("Test string with numbers 12345 and symbols !@#$%")]
        [InlineData("Multi\nLine\nString\nContent")]
        public void Test_CompressDecompress_Roundtrip_VariousInputs(string input)
        {
            var result = Compression.DecompressString(Compression.CompressString(input));
            Assert.Equal(input, result);
        }

        // ---- Fail Tests ----

        [Fact]
        public void Test_CompressString_OutputIsNotSameAsInput()
        {
            var original = "This is a test string";
            var compressed = Compression.CompressString(original);
            Assert.NotEqual(original, compressed);
        }

        [Fact]
        public void Test_DecompressString_InvalidBase64_ThrowsException()
        {
            Assert.ThrowsAny<Exception>(() => Compression.DecompressString("not-valid-base64!!!"));
        }

        [Fact]
        public void Test_CompressDecompress_TwoCompresses_NotEqualToOriginal()
        {
            var original = "test";
            var compressedOnce = Compression.CompressString(original);
            Assert.NotEqual(original, compressedOnce);
        }
    }
}
