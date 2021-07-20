using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using System.IO;
using System.Security.Cryptography;

namespace Encrypter
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        enum ENCRYPTION_TYPE
        {
            AES,EFS
        }

        ENCRYPTION_TYPE encryption;
        string[] files = new string[] { };
        public Form1()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(dragEnter);
            this.DragDrop += new DragEventHandler(dragDrop);

        }

        void dragEnter(object sender, DragEventArgs args)
        {
            if (args.Data.GetDataPresent(DataFormats.FileDrop))
                args.Effect = DragDropEffects.Copy;
        }

        void dragDrop(object sender, DragEventArgs args)
        {
           // bool counter;
           // args.Data.
           files  = (string[])args.Data.GetData(DataFormats.FileDrop);
            foreach (var file in files)
            {
               // if (label1.Text.Contains("Empty")) label1.Text = "";
                label1.Text += Path.GetFullPath(file) + "\n";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach(var file in files)
            {
               if((File.GetAttributes(Path.GetFullPath(file)) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    foreach (var path in Directory.EnumerateFiles(Path.GetFullPath(file), "*.*", SearchOption.AllDirectories))
                    {
                        switch (encryption)
                        {
                            case ENCRYPTION_TYPE.EFS:File.Encrypt(path); break;
                            case ENCRYPTION_TYPE.AES:
                                {
                                    var file_content = System.IO.File.ReadAllText(path);
                                    EncryptAesManaged(file_content);

                                    break;
                                }
                        }
                        
                    }
                }
                else
                {
                    switch (encryption)
                    {
                        case ENCRYPTION_TYPE.EFS: File.Encrypt(Path.GetFullPath(file)); break;
                        case ENCRYPTION_TYPE.AES:
                            {
                                var file_content = System.IO.File.ReadAllText(Path.GetFullPath(file));
                                EncryptAesManaged(file_content);
                                break;
                            }
                    }
                }
    
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var file in files)
            {
                if ((File.GetAttributes(Path.GetFullPath(file)) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    foreach (var path in Directory.EnumerateFiles(Path.GetFullPath(file), "*.*", SearchOption.AllDirectories))
                        File.Decrypt(path);
                }
                else
                {

                    File.Decrypt(Path.GetFullPath(file));
                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Array.Clear(files, 0, files.Length);
            label1.Text = "";
           // label1.auto
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
          //  if (comboBox1.Text == "")
          //  {
                //comboBox1.Text = "Select";
          //      button1.Enabled = false;
           // }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string defText = "Select encryption algorithm";
           // if (comboBox1.SelectedText != defText ) comboBox1.Items.Remove(defText);
        }


        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
        static void EncryptAesManaged(string raw)
        {
            try
            {
                // Create Aes that generates a new key and initialization vector (IV).    
                // Same key must be used in encryption and decryption    
                using (AesManaged aes = new AesManaged())
                {
                    // Encrypt string    
                    byte[] encrypted = Encrypt(raw, aes.Key, aes.IV);
                    // Print encrypted string    
                    Console.WriteLine($"Encrypted data: {System.Text.Encoding.UTF8.GetString(encrypted)}");
                    // Decrypt the bytes to a string.    
                    string decrypted = Decrypt(encrypted, aes.Key, aes.IV);
                    // Print decrypted string. It should be same as raw data    
                    Console.WriteLine($"Decrypted data: {decrypted}");
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
           // Console.ReadKey();
        }
        static byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            // Create a new AesManaged.    
            using (AesManaged aes = new AesManaged())
            {
                // Create encryptor    
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                // Create MemoryStream    
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        // Create StreamWriter and write data to a stream    
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
        }
        static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            // Create AesManaged    
            using (AesManaged aes = new AesManaged())
            {
                // Create a decryptor    
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption.    
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream    
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream    
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) encryption = ENCRYPTION_TYPE.AES;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked) encryption = ENCRYPTION_TYPE.EFS;
        }
    }
}
