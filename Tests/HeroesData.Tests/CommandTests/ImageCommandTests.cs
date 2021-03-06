﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.Tests.CommandTests
{
    [TestClass]
    public class ImageCommandTests
    {
        private readonly string _pngImage = "storm_ui_icon_abathur_symbiote.png";

        [TestMethod]
        public void NoArgumentsTest()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "image" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual("Must provide a file name, relative file path, or directory.", lines[0]);
        }

        [TestMethod]
        public void InvalidImagePathTest()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "image", "DoesNotExists.jpg" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual("File or directory does not exist.", lines[0]);
        }

        [DataTestMethod]
        [DataRow("-1")]
        [DataRow("-5")]
        [DataRow("0")]
        public void InvalidWidthOptionTest(string value)
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "image", Path.Combine("CommandTests", _pngImage), "--width", value });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual("Invalid width. Must be an integer greater than 0.", lines[0]);
        }

        [DataTestMethod]
        [DataRow("-1")]
        [DataRow("-5")]
        [DataRow("0")]
        public void InvalidHeightOptionTest(string value)
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "image", Path.Combine("CommandTests", _pngImage), "--height", value });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual("Invalid height. Must be an integer greater than 0.", lines[0]);
        }

        [TestMethod]
        public void ImageProcessedTest()
        {
            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Console.SetError(writer);

                Program.Main(new string[] { "image", Path.Combine("CommandTests", _pngImage) });

                List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

                Assert.AreEqual("Image processed.", lines[0]);
            }

            using Image image = Image.Load(Path.Combine("CommandTests", _pngImage));

            Assert.AreEqual(128, image.Width);
            Assert.AreEqual(128, image.Height);
        }

        [TestMethod]
        public void ImageNotProcessedTest()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "image", Path.Combine("CommandTests", "Test.txt") });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual("Image did not get processed.", lines[0]);
        }

        [TestMethod]
        public void DirectoryImagesProcessedTest()
        {
            using StringWriter writer = new StringWriter();

            Console.SetOut(writer);
            Console.SetError(writer);

            Program.Main(new string[] { "image", "CommandTests", "-o", "MultiImages", "--height", "64" });

            List<string> lines = writer.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual("\rProcessed 1\rProcessed 2", lines[0]);

            using Image image = Image.Load(Path.Combine("MultiImages", _pngImage));

            Assert.AreEqual(128, image.Width);
            Assert.AreEqual(64, image.Height);
        }
    }
}
