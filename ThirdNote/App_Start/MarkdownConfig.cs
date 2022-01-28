using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Markdig;
using Markdig.SyntaxHighlighting;

namespace ThirdNote.App_Start
{
    public class MarkdownConfig
    {
        public static MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions().UseYamlFrontMatter().UseEmojiAndSmiley().Build();
            //.UseTaskLists()
            //.UseMediaLinks()
            //.UseCustomContainers()
            //.UseYamlFrontMatter()
            //.UseDefinitionLists()
            //.UseGenericAttributes()
            //.UseGlobalization()
            //.UseAdvancedExtensions()
            //.UseEmphasisExtras()
            //.UseSyntaxHighlighting()
            //.Build();

        public static MarkdownPipeline GetPipeline()
        {
            
            return pipeline;
        }
    }
}