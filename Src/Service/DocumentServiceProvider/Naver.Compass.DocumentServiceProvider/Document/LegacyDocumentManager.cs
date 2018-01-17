using System;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace Naver.Compass.Service.Document
{
    internal class LegacyDocumentManager
    {
        internal LegacyDocumentManager(Document document)
        {
            Debug.Assert(document != null);

            // Do not user data in DocumentData, it is not loaded right now.
            _document = document;
        }

        // At this time, the working directory must be initialized, update document will only update the files in working directory, 
        // not the zipped pn file. Sometime we update major version just to make old product unable to open new version file, although file format is not changed.
        // Return true means document data is updated.
        internal bool UpdateToThisFileVersion()
        {
            Debug.Assert(_document.WorkingDirectory != null);
            Debug.Assert(_document.VersionData != null);

            _fileVersion = new Version(_document.VersionData.FileVersion);
            if (_fileVersion.Major < 6)
            {
                UpdateToV6();
            }

            if (_fileVersion.Major < 8)
            {
                UpdateFromV6ToV8();
            }

            UpdateFromV8ToV9();

            return true;
        }

        // Update file to version 6.0.0.0. See 6.0.0.0 changes in VersionHistory.txt
        private void UpdateToV6()
        {
            string documentXmlFile = Path.Combine(_document.WorkingDirectory.FullName, Document.DOCUMENT_FILE_NAME);
            XmlDocument oldXmlDoc = new XmlDocument();
            oldXmlDoc.Load(documentXmlFile);
            XmlElement oldDocElement = oldXmlDoc.DocumentElement;

            XmlDocument newXmlDoc = new XmlDocument();
            XmlElement newDocElement = newXmlDoc.CreateElement("Document");
            newXmlDoc.AppendChild(newDocElement);

            string documentGuid = oldDocElement.GetAttribute("Guid");
            newDocElement.SetAttribute("Guid", documentGuid);
            XmlNode importedNode = newXmlDoc.ImportNode(oldDocElement["DocumentType"], true);
            newDocElement.AppendChild(importedNode);

            XmlElement fileVersionElement = newXmlDoc.CreateElement("FileVersion");
            newDocElement.AppendChild(fileVersionElement);
            fileVersionElement.InnerText = VersionData.THIS_FILE_VERSION;

            // Pages
            XmlElement newPagesElement = newXmlDoc.CreateElement("Pages");
            newDocElement.AppendChild(newPagesElement);
            foreach (XmlElement pageElement in oldDocElement["Pages"].ChildNodes)
            {
                XmlElement newPageElement = newXmlDoc.CreateElement("StandardPage");
                newPagesElement.AppendChild(newPageElement);
                    
                newPageElement.SetAttribute("Guid", pageElement.GetAttribute("Guid"));
                importedNode = newXmlDoc.ImportNode(pageElement["Name"], true);
                newPageElement.AppendChild(importedNode);

                SavePageDataToXml(pageElement, "StandardPage");
            }

            // PageAnnotationFieldSet
            importedNode = newXmlDoc.ImportNode(oldDocElement["PageAnnotationFieldSet"], true);
            newDocElement.AppendChild(importedNode);

            // WidgetAnnotationFieldSet
            importedNode = newXmlDoc.ImportNode(oldDocElement["WidgetAnnotationFieldSet"], true);
            newDocElement.AppendChild(importedNode);

            // WidgetDefaultStyleSet
            XmlElement newSetElement = newXmlDoc.CreateElement("WidgetDefaultStyleSet");
            newDocElement.AppendChild(newSetElement);

            XmlElement newStylesElement = newXmlDoc.CreateElement("WidgetDefaultStyles");
            newSetElement.AppendChild(newStylesElement);

            foreach (XmlElement widgetStyleElement in oldDocElement["WidgetBaseStyleSet"]["DefaultWidgetBaseStlyes"].ChildNodes)
            {
                XmlElement newStyleElement = newXmlDoc.CreateElement("WidgetDefaultStyle");
                newStylesElement.AppendChild(newStyleElement);

                newStyleElement.SetAttribute("Guid", widgetStyleElement.GetAttribute("Guid"));

                // XmlElement Name property is read only, so ...
                foreach (XmlElement childElement in widgetStyleElement.ChildNodes)
                {
                    if(string.CompareOrdinal(childElement.Name, "AdaptiveViewGuid") == 0)
                    {
                        continue;
                    }

                    importedNode = newXmlDoc.ImportNode(childElement, true);
                    newStyleElement.AppendChild(importedNode);
                }
            }

            // Guides 
            importedNode = newXmlDoc.ImportNode(oldDocElement["Guides"], true);
            newDocElement.AppendChild(importedNode);

            // GeneratorConfigurationSet 
            importedNode = newXmlDoc.ImportNode(oldDocElement["GeneratorConfigurationSet"], true);
            newDocElement.AppendChild(importedNode);

            // AdaptiveViewSet 
            importedNode = newXmlDoc.ImportNode(oldDocElement["AdaptiveViewSet"], true);
            newDocElement.AppendChild(importedNode);

            // DeviceSet
            importedNode = newXmlDoc.ImportNode(oldDocElement["DeviceSet"], true);
            newDocElement.AppendChild(importedNode);

            // DocumentSettings
            importedNode = newXmlDoc.ImportNode(oldDocElement["DocumentSettings"], true);
            newDocElement.AppendChild(importedNode);

            // Save new xml document back to Document.xml
            string docXmlFileName = Path.Combine(_document.WorkingDirectory.FullName, Document.DOCUMENT_FILE_NAME);
            newXmlDoc.PreserveWhitespace = true;
            newXmlDoc.Save(docXmlFileName);

            // Save Version.xml
            string versionXmlFileName = Path.Combine(_document.WorkingDirectory.FullName, Document.VERSION_FILE_NAME);
            _document.VersionData.UpdateToCurrentVersion();
            _document.VersionData.Save(versionXmlFileName);

            // Save document guid file
            string docGuidFile = Path.Combine(_document.WorkingDirectory.FullName, documentGuid + DOCUMENT_GUID_SUFFIX);
            using (FileStream fs = File.Create(docGuidFile))
            {
            }

            // Delete old images folder
            string oldImagesFolder = Path.Combine(_document.WorkingDirectory.FullName, Document.IMAGES_FOLDER_NAME);
            if(Directory.Exists(oldImagesFolder))
            {
                Directory.Delete(oldImagesFolder, true);
            }
        }

        private void SavePageDataToXml(XmlElement pageElement, string pageTagName)
        {
            XmlDocument pageXmlDoc = new XmlDocument();
            XmlElement newPageElement = pageXmlDoc.CreateElement(pageTagName);
            pageXmlDoc.AppendChild(newPageElement);

            string pageGuid = pageElement.GetAttribute("Guid");

            XmlElement fileVersionElement = pageXmlDoc.CreateElement("FileVersion");
            newPageElement.AppendChild(fileVersionElement);
            fileVersionElement.InnerText = VersionData.THIS_FILE_VERSION;

            newPageElement.SetAttribute("Guid", pageGuid);
            XmlNode importedNode = pageXmlDoc.ImportNode(pageElement["Name"], true);
            newPageElement.AppendChild(importedNode);

            DirectoryInfo pageDir = _document.WorkingPagesDirectory.CreateSubdirectory(pageGuid); // Create page working directory in "pages"
            DirectoryInfo pageImagesDir = pageDir.CreateSubdirectory(Document.IMAGES_FOLDER_NAME); // Create "images" in page directory

            // Widgets
            importedNode = pageXmlDoc.ImportNode(pageElement["Widgets"], true);
            newPageElement.AppendChild(importedNode);
            foreach (XmlElement widgetElement in newPageElement["Widgets"].ChildNodes)
            {
                try
                {
                    string imageName = "";
                    if (string.CompareOrdinal(widgetElement.Name, "Image") == 0)
                    {
                        imageName = widgetElement.GetAttribute("Guid") + @".png";
                    }
                    else if (string.CompareOrdinal(widgetElement.Name, "Svg") == 0)
                    {
                        imageName = widgetElement.GetAttribute("Guid") + @".svg";
                    }
                    else if (string.CompareOrdinal(widgetElement.Name, "HamburgerMenu") == 0) //menu button
                    {
                        imageName = widgetElement.GetAttribute("Guid") + @".png";
                    }

                    if(!string.IsNullOrEmpty(imageName))
                    {
                        string sourceFileName = Path.Combine(_document.WorkingDirectory.FullName, Document.IMAGES_FOLDER_NAME, imageName);
                        string destFileName = Path.Combine(pageImagesDir.FullName, imageName);
                        if (File.Exists(sourceFileName) && !File.Exists(destFileName))
                        {
                            File.Move(sourceFileName, destFileName);
                        }
                    }

                    if (string.CompareOrdinal(widgetElement.Name, "Toast") == 0)
                    {
                        XmlElement oldToastPage = widgetElement["ToastPage"];
                        string toastPageGuid = oldToastPage.GetAttribute("Guid");
                        SaveEmbeddedPageDataToXml(pageDir, oldToastPage);
                        widgetElement.RemoveChild(oldToastPage);

                        XmlElement newToastPage = pageXmlDoc.CreateElement("ToastPage");
                        widgetElement.AppendChild(newToastPage);
                        newToastPage.SetAttribute("Guid", toastPageGuid);

                        XmlElement newNamePage = pageXmlDoc.CreateElement("Name");
                        newToastPage.AppendChild(newNamePage);
                        if (oldToastPage["Name"] != null)
                        {
                            newNamePage.InnerText = oldToastPage["Name"].InnerText;
                        }
                    }
                    else if (string.CompareOrdinal(widgetElement.Name, "HamburgerMenu") == 0)
                    {
                        XmlElement oldMenuPage = widgetElement["HamburgerMenuPage"];
                        string menuPageGuid = oldMenuPage.GetAttribute("Guid");
                        SaveEmbeddedPageDataToXml(pageDir, oldMenuPage);
                        widgetElement.RemoveChild(oldMenuPage);

                        XmlElement newMenuPage = pageXmlDoc.CreateElement("HamburgerMenuPage");
                        widgetElement.AppendChild(newMenuPage);
                        newMenuPage.SetAttribute("Guid", menuPageGuid);

                        XmlElement newNamePage = pageXmlDoc.CreateElement("Name");
                        newMenuPage.AppendChild(newNamePage);
                        if (oldMenuPage["Name"] != null)
                        {
                            newNamePage.InnerText = oldMenuPage["Name"].InnerText;
                        }

                        XmlElement menuButtonElement = widgetElement["HamburgerMenuButton"];
                        UpdateWidgetLocationAndSizeStyleProperties(pageXmlDoc, menuButtonElement);
                    }
                    else if (string.CompareOrdinal(widgetElement.Name, "DynamicPanel") == 0)
                    {
                        XmlElement oldStates = widgetElement["States"];

                        XmlElement newStates = pageXmlDoc.CreateElement("States");
                        widgetElement.AppendChild(newStates);

                        foreach (XmlElement oldStatePageElement in oldStates.ChildNodes)
                        {
                            string statePageGuid = oldStatePageElement.GetAttribute("Guid");
                            SaveEmbeddedPageDataToXml(pageDir, oldStatePageElement);

                            XmlElement newStatePage = pageXmlDoc.CreateElement("PanelStatePage");
                            newStates.AppendChild(newStatePage);
                            newStatePage.SetAttribute("Guid", statePageGuid);

                            XmlElement newNamePage = pageXmlDoc.CreateElement("Name");
                            newStatePage.AppendChild(newNamePage);
                            if (oldStatePageElement["Name"] != null)
                            {
                                newNamePage.InnerText = oldStatePageElement["Name"].InnerText;
                            }
                        }

                        widgetElement.RemoveChild(oldStates);                        
                    }


                    // Move location and size to <WidgetStyle>, see DOM version 1.4.0.0.
                    UpdateWidgetLocationAndSizeStyleProperties(pageXmlDoc, widgetElement);
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp.Message);
                }

            }

            // Annotation
            importedNode = pageXmlDoc.ImportNode(pageElement["Annotation"], true);
            newPageElement.AppendChild(importedNode);

            // Groups
            importedNode = pageXmlDoc.ImportNode(pageElement["Groups"], true);
            newPageElement.AppendChild(importedNode);

            // PageViews. There is no "PageViews" before v1.0.1 .
            if (pageElement["PageViews"] != null)
            {
                importedNode = pageXmlDoc.ImportNode(pageElement["PageViews"], true);
                newPageElement.AppendChild(importedNode);
            }

            // Save page data to xml file.
            string pageDataFileName = Path.Combine(pageDir.FullName, Document.PAGE_FILE_NAME);
            pageXmlDoc.PreserveWhitespace = true;
            pageXmlDoc.Save(pageDataFileName);
        }

        // Embedded page cannot add IPageEmbeddedWidget
        private void SaveEmbeddedPageDataToXml(DirectoryInfo parentPageWorkingDirectory, XmlElement embeddedPageElement)
        {
            XmlDocument pageXmlDoc = new XmlDocument();
            XmlNode importedNode = pageXmlDoc.ImportNode(embeddedPageElement, true);
            pageXmlDoc.AppendChild(importedNode);

            XmlElement newPageElement = pageXmlDoc.DocumentElement;
            XmlElement fileVersionElement = pageXmlDoc.CreateElement("FileVersion");
            newPageElement.InsertBefore(fileVersionElement, newPageElement.FirstChild);
            fileVersionElement.InnerText = VersionData.THIS_FILE_VERSION;

            // Remove page style
            XmlElement pageStyleElement = importedNode["PageStyle"];
            if (pageStyleElement != null)
            {
                importedNode.RemoveChild(pageStyleElement);
            }

            string pageGuid = embeddedPageElement.GetAttribute("Guid");

            DirectoryInfo pageDir = parentPageWorkingDirectory.CreateSubdirectory(pageGuid); // Create embedded page working directory in parent page directory.
            DirectoryInfo pageImagesDir = pageDir.CreateSubdirectory(Document.IMAGES_FOLDER_NAME); // Create "images" in page directory

            foreach (XmlElement widgetElement in importedNode["Widgets"].ChildNodes)
            {
                try
                {
                    string imageName = "";
                    if (string.CompareOrdinal(widgetElement.Name, "Image") == 0)
                    {
                        imageName = widgetElement.GetAttribute("Guid") + @".png";
                    }
                    else if (string.CompareOrdinal(widgetElement.Name, "Svg") == 0)
                    {
                        imageName = widgetElement.GetAttribute("Guid") + @".svg";
                    }

                    if (!string.IsNullOrEmpty(imageName))
                    {
                        string sourceFileName = Path.Combine(_document.WorkingDirectory.FullName, Document.IMAGES_FOLDER_NAME, imageName);
                        string destFileName = Path.Combine(pageImagesDir.FullName, imageName);
                        if (File.Exists(sourceFileName) && !File.Exists(destFileName))
                        {
                            File.Move(sourceFileName, destFileName);
                        }
                    }

                    UpdateWidgetLocationAndSizeStyleProperties(pageXmlDoc, widgetElement);
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp.Message);
                }

            }

            // Save page data to xml file.
            string pageDataFileName = Path.Combine(pageDir.FullName, Document.PAGE_FILE_NAME);
            pageXmlDoc.PreserveWhitespace = true;
            pageXmlDoc.Save(pageDataFileName);
        }

        private void UpdateWidgetLocationAndSizeStyleProperties(XmlDocument pageXmlDoc, XmlElement widgetElement)
        {
            // Move location and size to <WidgetStyle>, see DOM version 1.4.0.0.
            XmlElement propertiesElement = widgetElement["Properties"];
            XmlElement widgetStyleElement = widgetElement["WidgetStyle"];
            if (propertiesElement != null && widgetStyleElement != null)
            {
                XmlElement isVisibleElement = propertiesElement["IsVisible"];
                XmlElement xElement = propertiesElement["X"];
                XmlElement yElement = propertiesElement["Y"];
                XmlElement heightElement = propertiesElement["Height"];
                XmlElement widthElement = propertiesElement["Width"];
                XmlElement zElement = propertiesElement["Z"];

                if (isVisibleElement != null)
                {
                    if (String.CompareOrdinal(isVisibleElement.InnerText, "False") == 0
                    && widgetStyleElement["IsVisibleProp"] == null)
                    {
                        XmlElement isVisibleInStyleElement = pageXmlDoc.CreateElement("IsVisibleProp");
                        widgetStyleElement.AppendChild(isVisibleInStyleElement);

                        XmlElement valueElement = pageXmlDoc.CreateElement("Value");
                        isVisibleInStyleElement.AppendChild(valueElement);
                        valueElement.InnerText = isVisibleElement.InnerText;

                        XmlElement overriddenElement = pageXmlDoc.CreateElement("Overridden");
                        isVisibleInStyleElement.AppendChild(overriddenElement);
                        overriddenElement.InnerText = "True";
                    }

                    propertiesElement.RemoveChild(isVisibleElement);
                }

                if (xElement != null)
                {
                    if(widgetStyleElement["XProp"] == null)
                    {
                        XmlElement xElementInStyleElement = pageXmlDoc.CreateElement("XProp");
                        widgetStyleElement.AppendChild(xElementInStyleElement);

                        XmlElement valueElement = pageXmlDoc.CreateElement("Value");
                        xElementInStyleElement.AppendChild(valueElement);
                        valueElement.InnerText = xElement.InnerText;

                        XmlElement overriddenElement = pageXmlDoc.CreateElement("Overridden");
                        xElementInStyleElement.AppendChild(overriddenElement);
                        overriddenElement.InnerText = "True";
                    }

                    propertiesElement.RemoveChild(xElement);
                }

                if (yElement != null)
                {
                    if (widgetStyleElement["YProp"] == null)
                    {
                        XmlElement yElementInStyleElement = pageXmlDoc.CreateElement("YProp");
                        widgetStyleElement.AppendChild(yElementInStyleElement);

                        XmlElement valueElement = pageXmlDoc.CreateElement("Value");
                        yElementInStyleElement.AppendChild(valueElement);
                        valueElement.InnerText = yElement.InnerText;

                        XmlElement overriddenElement = pageXmlDoc.CreateElement("Overridden");
                        yElementInStyleElement.AppendChild(overriddenElement);
                        overriddenElement.InnerText = "True";
                    }

                    propertiesElement.RemoveChild(yElement);
                }

                if (heightElement != null)
                {
                    if (widgetStyleElement["HeightProp"] == null)
                    {
                        XmlElement heightElementInStyleElement = pageXmlDoc.CreateElement("HeightProp");
                        widgetStyleElement.AppendChild(heightElementInStyleElement);

                        XmlElement valueElement = pageXmlDoc.CreateElement("Value");
                        heightElementInStyleElement.AppendChild(valueElement);
                        valueElement.InnerText = heightElement.InnerText;

                        XmlElement overriddenElement = pageXmlDoc.CreateElement("Overridden");
                        heightElementInStyleElement.AppendChild(overriddenElement);
                        overriddenElement.InnerText = "True";
                    }

                    propertiesElement.RemoveChild(heightElement);
                }

                if (widthElement != null)
                {
                    if (widgetStyleElement["WidthProp"] == null)
                    {
                        XmlElement widthElementInStyleElement = pageXmlDoc.CreateElement("WidthProp");
                        widgetStyleElement.AppendChild(widthElementInStyleElement);

                        XmlElement valueElement = pageXmlDoc.CreateElement("Value");
                        widthElementInStyleElement.AppendChild(valueElement);
                        valueElement.InnerText = widthElement.InnerText;

                        XmlElement overriddenElement = pageXmlDoc.CreateElement("Overridden");
                        widthElementInStyleElement.AppendChild(overriddenElement);
                        overriddenElement.InnerText = "True";
                    }

                    propertiesElement.RemoveChild(widthElement);
                }

                if (zElement != null)
                {
                    if (widgetStyleElement["ZProp"] == null)
                    {
                        XmlElement zElementInStyleElement = pageXmlDoc.CreateElement("ZProp");
                        widgetStyleElement.AppendChild(zElementInStyleElement);

                        XmlElement valueElement = pageXmlDoc.CreateElement("Value");
                        zElementInStyleElement.AppendChild(valueElement);
                        valueElement.InnerText = zElement.InnerText;

                        XmlElement overriddenElement = pageXmlDoc.CreateElement("Overridden");
                        zElementInStyleElement.AppendChild(overriddenElement);
                        overriddenElement.InnerText = "True";
                    }

                    propertiesElement.RemoveChild(zElement);
                }
            }
        }

        // Update file from version 6.0.0.0 to version 8.0.0.0. See 8.0.0.0 changes in VersionHistory.txt
        private void UpdateFromV6ToV8()
        {
            string documentXmlFile = Path.Combine(_document.WorkingDirectory.FullName, Document.DOCUMENT_FILE_NAME);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(documentXmlFile);
            XmlElement docElement = xmlDoc.DocumentElement;

            // Update <FileVersion> 
            // Create <FileVersion> if there is no such element.
            // See http://bts1.navercorp.com/nhnbts/browse/DSTUDIO-2023
            if (docElement["FileVersion"] == null)
            {
                XmlElement fileVersionElement = xmlDoc.CreateElement("FileVersion");
                docElement.InsertBefore(fileVersionElement, docElement.FirstChild);
            }

            docElement["FileVersion"].InnerText = VersionData.THIS_FILE_VERSION;

            // WidgetDefaultStyleSet
            XmlElement oldWidgetDefaultStylesElement = docElement["WidgetDefaultStyleSet"]["WidgetDefaultStyles"];

            // Remove old one from document
            docElement["WidgetDefaultStyleSet"].RemoveChild(oldWidgetDefaultStylesElement);

            // Create a new one
            XmlElement newWidgetDefaultStylesElement = xmlDoc.CreateElement("WidgetDefaultStyles");
            docElement["WidgetDefaultStyleSet"].AppendChild(newWidgetDefaultStylesElement);

            foreach (XmlElement oldStyleElement in oldWidgetDefaultStylesElement.ChildNodes)
            {
                XmlElement newStyleElement = xmlDoc.CreateElement(oldStyleElement.Name);
                newWidgetDefaultStylesElement.AppendChild(newStyleElement);

                XmlElement nameElement = xmlDoc.CreateElement("Name");
                nameElement.InnerText = oldStyleElement["Name"].InnerText;
                newStyleElement.AppendChild(nameElement);

                XmlElement styleProperties = xmlDoc.CreateElement("StyleProperties");
                newStyleElement.AppendChild(styleProperties);

                bool hadBeenSet = true;
                foreach (XmlElement oldPropElement in oldStyleElement.ChildNodes)
                {
                    if (String.CompareOrdinal(oldPropElement.Name, "Name") == 0)
                    {
                        continue;
                    }
                    else if (WidgetSupportedStylePropertyCheckWithDefaultStyleName(nameElement.InnerText, oldPropElement.Name))
                    {

                        // Before DOM 5.1.0.0 (protoNow 1.4.1), the Overridden is False event we had set the default value,
                        // so we have to handle this.
                        if (_fileVersion.Major < 5 || (_fileVersion.Major == 5 && _fileVersion.Minor == 0))
                        {
                            XmlElement newPropElement = xmlDoc.CreateElement(oldPropElement.Name);
                            styleProperties.AppendChild(newPropElement);

                            XmlElement oldValueElement = oldPropElement["Value"];
                            if (oldValueElement != null)
                            {
                                XmlElement newValueElement = xmlDoc.CreateElement("Value");
                                newPropElement.AppendChild(newValueElement);
                                newValueElement.InnerText = oldValueElement.InnerText;
                            }
                            else
                            {
                                XmlElement oldColorElement = oldPropElement["Color"];
                                XmlNode newColorElement = xmlDoc.ImportNode(oldColorElement, true);
                                newPropElement.AppendChild(newColorElement);
                            }

                        }
                        else
                        {
                            // Only keep overridden is true elements.                    
                            XmlElement oldOverriddenElement = oldPropElement["Overridden"];
                            bool overridden = false;
                            if (Boolean.TryParse(oldOverriddenElement.InnerText, out overridden))
                            {
                                if (overridden)
                                {
                                    XmlElement newPropElement = xmlDoc.CreateElement(oldPropElement.Name);
                                    styleProperties.AppendChild(newPropElement);

                                    XmlElement oldValueElement = oldPropElement["Value"];
                                    if (oldValueElement != null)
                                    {
                                        XmlElement newValueElement = xmlDoc.CreateElement("Value");
                                        newPropElement.AppendChild(newValueElement);
                                        newValueElement.InnerText = oldValueElement.InnerText;
                                    }
                                    else
                                    {
                                        XmlElement oldColorElement = oldPropElement["Color"];
                                        XmlNode newColorElement = xmlDoc.ImportNode(oldColorElement, true);
                                        newPropElement.AppendChild(newColorElement);
                                    }
                                }
                                else
                                {
                                    hadBeenSet = false;
                                    break;
                                }
                            }
                        }
                    }
                }

                // Remove all styles if the default style has not been set before.
                if (!hadBeenSet)
                {
                    styleProperties.RemoveAll();
                }
            }

            // Save xml document back to Document.xml
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Save(documentXmlFile);

            // Save Version.xml
            string versionXmlFileName = Path.Combine(_document.WorkingDirectory.FullName, Document.VERSION_FILE_NAME);
            _document.VersionData.UpdateToCurrentVersion();
            _document.VersionData.Save(versionXmlFileName);

            // Delete document guid file
            try
            {
                string docGuidFile = Path.Combine(_document.WorkingDirectory.FullName, docElement.GetAttribute("Guid") + DOCUMENT_GUID_SUFFIX);
                File.Delete(docGuidFile);
            }
            catch(Exception exp)
            {
                Debug.WriteLine(exp.Message);
            }

            // Update Page.Xml
            foreach (DirectoryInfo pageDir in _document.WorkingPagesDirectory.EnumerateDirectories("*", SearchOption.AllDirectories))
            {
                Guid guid;
                if (Guid.TryParse(pageDir.Name, out guid))
                {
                    try
                    {
                        UpdatePageXml(pageDir.FullName);
                    }
                    catch(Exception exp)
                    {
                        // Keep the bad page data without updating it.
                        Debug.WriteLine(exp.Message);
                    }
                }
            }
        }

        private void UpdatePageXml(string pageDir)
        {
            string pageXmlFile = Path.Combine(pageDir, Document.PAGE_FILE_NAME);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(pageXmlFile);
            XmlElement pageElement = xmlDoc.DocumentElement;

            // Update <FileVersion>
            pageElement["FileVersion"].InnerText = VersionData.THIS_FILE_VERSION;
            
            foreach (XmlElement widgetElement in pageElement["Widgets"].ChildNodes)
            {
                UpdateWidget(xmlDoc, widgetElement);

                // HamburgerMenu has a HamburgerMenuButton
                if (string.CompareOrdinal(widgetElement.Name, "HamburgerMenu") == 0)
                {
                    XmlElement menuButtonElement = widgetElement["HamburgerMenuButton"];
                    UpdateWidget(xmlDoc, menuButtonElement);
                }
            }

            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Save(pageXmlFile);
        }

        private void UpdateWidget(XmlDocument xmlDoc, XmlElement widgetElement)
        {
            // Move Widget <Name> out of <Properties>
            XmlElement oldNameElement = widgetElement["Properties"]["Name"];

            XmlElement newNameElement = xmlDoc.CreateElement("Name");
            newNameElement.InnerText = oldNameElement.InnerText;
            widgetElement.InsertBefore(newNameElement, widgetElement.FirstChild);

            widgetElement["Properties"].RemoveChild(oldNameElement);

            XmlElement oldWidgetStyleElement = widgetElement["WidgetStyle"];
            XmlNode nextSilbing = oldWidgetStyleElement.NextSibling;

            // Remove old one from document
            widgetElement.RemoveChild(oldWidgetStyleElement);

            // Create a new one
            XmlElement newWidgetStyleElement = xmlDoc.CreateElement("WidgetStyle");
            if (nextSilbing != null)
            {
                widgetElement.InsertBefore(newWidgetStyleElement, nextSilbing);
            }
            else
            {
                widgetElement.AppendChild(newWidgetStyleElement);
            }

            UpdateWidgetStyle(xmlDoc, newWidgetStyleElement, oldWidgetStyleElement, widgetElement.Name);

            XmlElement oldWidgetStylesElement = widgetElement["WidgetStyles"];

            if (oldWidgetStylesElement != null)
            {
                // Remove old one from document
                widgetElement.RemoveChild(oldWidgetStylesElement);

                // Create a new one
                XmlElement newWidgetStylesElement = xmlDoc.CreateElement("WidgetStyles");
                widgetElement.AppendChild(newWidgetStylesElement);

                foreach (XmlElement oldWidgetViewStyleElement in oldWidgetStylesElement.ChildNodes)
                {
                    XmlElement newWidgetViewStyleElement = xmlDoc.CreateElement("WidgetStyle");
                    newWidgetStylesElement.AppendChild(newWidgetViewStyleElement);

                    UpdateWidgetStyle(xmlDoc, newWidgetViewStyleElement, oldWidgetViewStyleElement, widgetElement.Name);
                }
            }
        }

        private void UpdateWidgetStyle(XmlDocument xmlDoc, XmlElement newWidgetStyleElement, 
                                       XmlElement oldWidgetStyleElement, string widgetElementName)
        {
            XmlElement viewElement = xmlDoc.CreateElement("AdaptiveViewGuid");
            if (oldWidgetStyleElement["AdaptiveViewGuid"] == null)
            {
                viewElement.InnerText = Guid.Empty.ToString();
            }
            else
            {
                viewElement.InnerText = oldWidgetStyleElement["AdaptiveViewGuid"].InnerText;
            }
            newWidgetStyleElement.AppendChild(viewElement);

            XmlElement styleProperties = xmlDoc.CreateElement("StyleProperties");
            newWidgetStyleElement.AppendChild(styleProperties);

            foreach (XmlElement oldPropElement in oldWidgetStyleElement.ChildNodes)
            {
                if (!oldPropElement.Name.EndsWith("Prop"))
                {
                    continue;
                }
                else 
                {
                    // Only keep overridden is true elements.                    
                    XmlElement oldOverriddenElement = oldPropElement["Overridden"];
                    bool overridden = false;
                    if (Boolean.TryParse(oldOverriddenElement.InnerText, out overridden))
                    {
                        if (overridden)
                        {
                            if (String.CompareOrdinal(oldPropElement.Name, "IsFixedProp") != 0
                                && String.CompareOrdinal(oldPropElement.Name, "IsVisibleProp") != 0
                                && String.CompareOrdinal(oldPropElement.Name, "XProp") != 0
                                && String.CompareOrdinal(oldPropElement.Name, "YProp") != 0
                                && String.CompareOrdinal(oldPropElement.Name, "HeightProp") != 0
                                && String.CompareOrdinal(oldPropElement.Name, "WidthProp") != 0
                                && String.CompareOrdinal(oldPropElement.Name, "ZProp") != 0)                                
                            {
                                if(!WidgetSupportedStylePropertyCheckWithWidgetTagName(widgetElementName, oldPropElement.Name))
                                {
                                    continue;
                                }
                            }

                            XmlElement newPropElement = xmlDoc.CreateElement(oldPropElement.Name);
                            styleProperties.AppendChild(newPropElement);

                            XmlElement oldValueElement = oldPropElement["Value"];
                            if (oldValueElement != null)
                            {
                                XmlElement newValueElement = xmlDoc.CreateElement("Value");
                                newPropElement.AppendChild(newValueElement);
                                newValueElement.InnerText = oldValueElement.InnerText;
                            }
                            else
                            {
                                XmlElement oldColorElement = oldPropElement["Color"];
                                XmlNode newColorElement = xmlDoc.ImportNode(oldColorElement, true);
                                newPropElement.AppendChild(newColorElement);
                            }
                        }
                    }
                }
            }           
        }

        private bool WidgetSupportedStylePropertyCheckWithDefaultStyleName(string widgetDefaultStyleName, string stylePropertyName)
        {
            switch (widgetDefaultStyleName)
            {
                case DefaultStyleNames.DEFAULT_FLOW_SHAPE_STYLE_NAME:
                    return FlowShape.SupportedStyleProperty(stylePropertyName);

                case DefaultStyleNames.DEFAULT_SHAPE_STYLE_NAME:
                case DefaultStyleNames.DEFAULT_SHAPE_RECTANGLE_STYLE_NAME:
                case DefaultStyleNames.DEFAULT_SHAPE_ROUNDED_RECTANGLE_STYLE_NAME:
                case DefaultStyleNames.DEFAULT_SHAPE_ELLIPSE_STYLE_NAME:
                case DefaultStyleNames.DEFAULT_SHAPE_DIAMOND_STYLE_NAME:
                case DefaultStyleNames.DEFAULT_SHAPE_TRIANGLE_STYLE_NAME:
                case DefaultStyleNames.DEFAULT_SHAPE_PARAGRAPH_STYLE_NAME:
                    return Shape.SupportedStyleProperty(stylePropertyName);

                case DefaultStyleNames.DEFAULT_IMAGE_STYLE_NAME:
                    return Image.SupportedStyleProperty(stylePropertyName);

                case DefaultStyleNames.DEFAULT_DYNAMICPANEL_STYLE_NAME:
                    return DynamicPanel.SupportedStyleProperty(stylePropertyName);

                case DefaultStyleNames.DEFAULT_HAMBURGERMENU_STYLE_NAME:
                    return HamburgerMenu.SupportedStyleProperty(stylePropertyName);

                case DefaultStyleNames.DEFAULT_TOAST_STYLE_NAME:
                    return Toast.SupportedStyleProperty(stylePropertyName);

                case DefaultStyleNames.DEFAULT_LINE_STYLE_NAME:
                    return Line.SupportedStyleProperty(stylePropertyName);

                case DefaultStyleNames.DEFAULT_HOTSPOT_STYLE_NAME:
                    return HotSpot.SupportedStyleProperty(stylePropertyName);

                case DefaultStyleNames.DEFAULT_TEXTFIELD_STYLE_NAME:
                    return TextField.SupportedStyleProperty(stylePropertyName);

                case DefaultStyleNames.DEFAULT_TEXTAREA_STYLE_NAME:
                    return TextArea.SupportedStyleProperty(stylePropertyName);

                case DefaultStyleNames.DEFAULT_DROPLIST_STYLE_NAME:
                    return Droplist.SupportedStyleProperty(stylePropertyName);

                case DefaultStyleNames.DEFAULT_LISTBOX_STYLE_NAME:
                    return ListBox.SupportedStyleProperty(stylePropertyName);

                case DefaultStyleNames.DEFAULT_CHECKBOX_STYLE_NAME:
                    return Checkbox.SupportedStyleProperty(stylePropertyName);

                case DefaultStyleNames.DEFAULT_RADIOBUTTON_STYLE_NAME:
                    return RadioButton.SupportedStyleProperty(stylePropertyName);

                case DefaultStyleNames.DEFAULT_BUTTON_STYLE_NAME:
                    return Button.SupportedStyleProperty(stylePropertyName);

                case DefaultStyleNames.DEFAULT_SVG_STYLE_NAME:
                    return Svg.SupportedStyleProperty(stylePropertyName);

                default:
                    return false;
            }
        }

        private bool WidgetSupportedStylePropertyCheckWithWidgetTagName(string widgetElementName, string stylePropertyName)
        {
            switch (widgetElementName)
            {
                case "FlowShape":
                    return FlowShape.SupportedStyleProperty(stylePropertyName);

                case "Shape":
                    return Shape.SupportedStyleProperty(stylePropertyName);

                case "Image":
                case "HamburgerMenuButton":
                    return Image.SupportedStyleProperty(stylePropertyName);

                case "DynamicPanel":
                    return DynamicPanel.SupportedStyleProperty(stylePropertyName);

                case "HamburgerMenu":
                    return HamburgerMenu.SupportedStyleProperty(stylePropertyName);

                case "Toast":
                    return Toast.SupportedStyleProperty(stylePropertyName);

                case "Line":
                    return Line.SupportedStyleProperty(stylePropertyName);

                case "HotSpot":
                    return HotSpot.SupportedStyleProperty(stylePropertyName);

                case "TextField":
                    return TextField.SupportedStyleProperty(stylePropertyName);

                case "TextArea":
                    return TextArea.SupportedStyleProperty(stylePropertyName);

                case "Droplist":
                    return Droplist.SupportedStyleProperty(stylePropertyName);

                case "ListBox":
                    return ListBox.SupportedStyleProperty(stylePropertyName);

                case "Checkbox":
                    return Checkbox.SupportedStyleProperty(stylePropertyName);

                case "RadioButton":
                    return RadioButton.SupportedStyleProperty(stylePropertyName);

                case "Button":
                    return Button.SupportedStyleProperty(stylePropertyName);

                case "Svg":
                    return Svg.SupportedStyleProperty(stylePropertyName);

                default:
                    return false;
            }
        }

        private void UpdateFromV8ToV9()
        {
            string documentXmlFile = Path.Combine(_document.WorkingDirectory.FullName, Document.DOCUMENT_FILE_NAME);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(documentXmlFile);
            XmlElement docElement = xmlDoc.DocumentElement;

            docElement["FileVersion"].InnerText = VersionData.THIS_FILE_VERSION;

            // Save xml document back to Document.xml
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Save(documentXmlFile);

            // Save Version.xml
            string versionXmlFileName = Path.Combine(_document.WorkingDirectory.FullName, Document.VERSION_FILE_NAME);
            _document.VersionData.UpdateToCurrentVersion();
            _document.VersionData.Save(versionXmlFileName);

            // Version before V6 has this images directory, but it will be deleted in UpdateToV6();
            // So make sure images directory exists.
            if(!_document.WorkingImagesDirectory.Exists)
            {
                _document.WorkingImagesDirectory.Create();
            }

            // Set Stream widget hash in Page.Xml
            foreach (DirectoryInfo pageDir in _document.WorkingPagesDirectory.EnumerateDirectories("*", SearchOption.AllDirectories))
            {
                Guid guid;
                if (Guid.TryParse(pageDir.Name, out guid))
                {
                    try
                    {
                        UpdateHashInPage(pageDir.FullName, guid);
                    }
                    catch (Exception exp)
                    {
                        // Keep the bad page data without updating it.
                        Debug.WriteLine(exp.Message);
                    }
                }
            }

            // Save Images.xml
            string imagesXmlFileName = Path.Combine(_document.WorkingDirectory.FullName, Document.IMAGES_FILE_NAME);
            _document.ImagesData.Save(imagesXmlFileName);
        }

        private void UpdateHashInPage(string pageDir, Guid pageGuid)
        {
            string pageXmlFile = Path.Combine(pageDir, Document.PAGE_FILE_NAME);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(pageXmlFile);
            XmlElement pageElement = xmlDoc.DocumentElement;

            // Update <FileVersion>
            pageElement["FileVersion"].InnerText = VersionData.THIS_FILE_VERSION;

            DirectoryInfo pageImageDirInfo = new DirectoryInfo(Path.Combine(pageDir, Document.IMAGES_FOLDER_NAME));
            if(pageImageDirInfo.Exists)
            {
                foreach (XmlElement widgetElement in pageElement["Widgets"].ChildNodes)
                {
                    if (string.CompareOrdinal(widgetElement.Name, "Image") == 0
                        || string.CompareOrdinal(widgetElement.Name, "Svg") == 0)
                    {
                        SetStreamWidgetHash(xmlDoc, widgetElement, pageImageDirInfo, pageGuid);
                    }
                    else if (string.CompareOrdinal(widgetElement.Name, "HamburgerMenu") == 0)
                    {
                        // HamburgerMenu has a HamburgerMenuButton
                        XmlElement menuButtonElement = widgetElement["HamburgerMenuButton"];
                        SetStreamWidgetHash(xmlDoc, menuButtonElement, pageImageDirInfo, pageGuid);
                    }
                }

                // Delete images directory in page directory.
                pageImageDirInfo.Delete(true);
            }

            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Save(pageXmlFile);
        }

        private void SetStreamWidgetHash(XmlDocument xmlDoc, XmlElement widgetElement, DirectoryInfo pageImageDirInfo,  Guid pageGuid)
        {
            Guid guid;
            string guidString = widgetElement.GetAttribute("Guid");
            if (Guid.TryParse(guidString, out guid))
            {
                FileInfo[] files = pageImageDirInfo.GetFiles(guidString + ".*");
                if (files.Length > 0)
                {
                    // The stream stored in ImagesStreamManager is a copy, so we can close the original stream here.
                    using (Stream stream = new MemoryStream())
                    {
                        using (FileStream fileStream = new FileStream(files[0].FullName, FileMode.Open, FileAccess.Read))
                        {
                            fileStream.CopyTo(stream);
                            stream.Position = 0;
                        }

                        // Check image format and make sure image type is correct.
                        if (string.CompareOrdinal(widgetElement.Name, "Svg") != 0)
                        {
                            XmlElement typeElement = widgetElement["ImageType"];
                            if (typeElement != null)
                            {
                                string type = typeElement.InnerText;
                                CheckImageType(stream, ref type);
                                typeElement.InnerText = type;
                                stream.Position = 0;
                            }
                        }

                        string extension = Path.GetExtension(files[0].Name);
                        if (extension.StartsWith(".") && extension.Length > 2)
                        {
                            extension = extension.Substring(1);
                        }
                        string hash = _document.ImagesStreamManager.SetStream(stream, extension);

                        XmlElement hashElement = xmlDoc.CreateElement("Hash");
                        hashElement.InnerText = hash;
                        widgetElement.InsertBefore(hashElement, widgetElement.FirstChild);

                        _document.ImagesData.AddConsumer(hash, guid, pageGuid);
                    }
                }
            }
        }

        private void CheckImageType(Stream imageStream, ref string type)
        {
            try
            {
                BitmapDecoder decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand);
                if (decoder is BmpBitmapDecoder) type = ImageType.BMP.ToString();
                else if (decoder is PngBitmapDecoder) type = ImageType.PNG.ToString();
                else if (decoder is JpegBitmapDecoder) type = ImageType.JPG.ToString();
                else if (decoder is GifBitmapDecoder) type = ImageType.GIF.ToString();
                else if (decoder is IconBitmapDecoder) type = ImageType.ICO.ToString();

                // Try to release BitmapDecoder
                decoder = null;
                GC.Collect();
            }
            catch(Exception exp)
            {
                Debug.WriteLine(exp.Message);
            }
        }

        private Document _document;
        private Version _fileVersion;

        private const string  DOCUMENT_GUID_SUFFIX = ".docguid";
    }
}
