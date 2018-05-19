using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Messenger
{
    public class MessagesTemplateSelector : DataTemplateSelector
    {
        public MessagesTemplateSelector()
        {

        }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element != null && item != null && item is API.Messages)
            {
                API.Messages message = item as API.Messages;

                var template = new DataTemplate();
                var stackPanel = new FrameworkElementFactory(typeof(StackPanel));
                var userName = new FrameworkElementFactory(typeof(TextBlock));
                var text = new FrameworkElementFactory(typeof(TextBlock));
                var attachmentsStackPanel = new FrameworkElementFactory(typeof(WrapPanel));
                var date = new FrameworkElementFactory(typeof(TextBlock));

                userName.SetBinding(TextBlock.TextProperty, new Binding("userId"));
                text.SetBinding(TextBlock.TextProperty, new Binding("text"));
                attachmentsStackPanel.SetValue(WrapPanel.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
                attachmentsStackPanel.SetValue(StackPanel.BackgroundProperty, Brushes.Transparent);
                if (message.attachments != null)
                {
                    for (var i = 0; i < message.attachments.Count; i++)
                    {
                        var attachment = new FrameworkElementFactory(typeof(Image));
                        attachment.SetBinding(Image.SourceProperty, new Binding($"attachments[{i}]"));
                        attachment.SetValue(Image.MaxWidthProperty, 100.0);
                        attachment.SetValue(Image.MaxHeightProperty, 100.0);
                        attachmentsStackPanel.AppendChild(attachment);
                    }
                }
                date.SetBinding(TextBlock.TextProperty, new Binding("date"));

                stackPanel.SetValue(StackPanel.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
                stackPanel.SetValue(StackPanel.ContextMenuProperty, element.FindResource("messageContextMenu"));
                stackPanel.SetBinding(StackPanel.NameProperty, new Binding("id"));

                stackPanel.AppendChild(userName);
                if (!String.IsNullOrWhiteSpace(message.text))
                {
                    stackPanel.AppendChild(text);
                }
                if (message.attachments != null)
                {
                    stackPanel.AppendChild(attachmentsStackPanel);
                }
                stackPanel.AppendChild(date);
                template.VisualTree = stackPanel;

                return template;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
