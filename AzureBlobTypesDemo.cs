using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // Replace with your connection string from Azure Storage Account → Access Keys
        string connectionString = "<Your_Connection_String>";
        string containerName = "demo-container";

        // Create container client
        BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
        await container.CreateIfNotExistsAsync();

        // ========================
        // 1️⃣ BLOCK BLOB DEMO
        // ========================
        Console.WriteLine("=== BLOCK BLOB ===");
        string blockBlobName = "sample-block.txt";
        BlobClient blockBlobClient = container.GetBlobClient(blockBlobName);

        // Upload text content
        await blockBlobClient.UploadAsync(BinaryData.FromString("Hello from Block Blob!"), overwrite: true);

        // Download & print
        var blockDownload = await blockBlobClient.DownloadContentAsync();
        Console.WriteLine("Block Blob Content: " + blockDownload.Value.Content.ToString());

        // ========================
        // 2️⃣ APPEND BLOB DEMO
        // ========================
        Console.WriteLine("\n=== APPEND BLOB ===");
        string appendBlobName = "sample-append.txt";
        AppendBlobClient appendBlobClient = container.GetAppendBlobClient(appendBlobName);

        // Create append blob if it doesn't exist
        await appendBlobClient.CreateIfNotExistsAsync();

        // Append two entries
        await appendBlobClient.AppendBlockAsync(new MemoryStream(Encoding.UTF8.GetBytes("First log entry\n")));
        await appendBlobClient.AppendBlockAsync(new MemoryStream(Encoding.UTF8.GetBytes("Second log entry\n")));

        // Download & print
        var appendDownload = await appendBlobClient.DownloadContentAsync();
        Console.WriteLine("Append Blob Content:\n" + appendDownload.Value.Content.ToString());

        // ========================
        // 3️⃣ PAGE BLOB DEMO
        // ========================
        Console.WriteLine("\n=== PAGE BLOB ===");
        string pageBlobName = "sample-page.vhd";
        PageBlobClient pageBlobClient = container.GetPageBlobClient(pageBlobName);

        // Create a 1 KB page blob (must be multiple of 512 bytes)
        long blobSize = 512 * 2; 
        await pageBlobClient.CreateIfNotExistsAsync(blobSize);

        // Prepare 512-byte aligned data
        byte[] pageData = Encoding.UTF8.GetBytes("Hello Page Blob!".PadRight(512));
        await pageBlobClient.UploadPagesAsync(new MemoryStream(pageData), 0);

        // Download & print raw content (will include padding spaces)
        var pageDownload = await pageBlobClient.DownloadContentAsync();
        Console.WriteLine("Page Blob Raw Data (with padding):");
        Console.WriteLine(pageDownload.Value.Content.ToString());

        Console.WriteLine("\n=== DONE! Check Azure Storage Explorer to see the files ===");
    }
}
