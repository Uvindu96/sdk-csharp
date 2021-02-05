﻿// Copyright 2021 Cloud Native Foundation. 
// Licensed under the Apache 2.0 license.
// See LICENSE file in the project root for full license information.

using System;
using System.Text;
using Xunit;

namespace CloudNative.CloudEvents.Extensions.UnitTests
{
    public class SamplingTest
    {
        private static readonly string sampleJson = @"
           {
               'specversion' : '1.0',
               'type' : 'com.github.pull.create',
               'id' : 'A234-1234-1234',
               'time' : '2018-04-05T17:31:00Z',
               'sampledrate' : 1,
           }".Replace('\'', '"');

        [Fact]
        public void SamplingParse()
        {
            var jsonFormatter = new JsonEventFormatter();
            var cloudEvent = jsonFormatter.DecodeStructuredEvent(Encoding.UTF8.GetBytes(sampleJson), Sampling.AllAttributes);

            Assert.Equal(1, cloudEvent["sampledrate"]);
            Assert.Equal(1, cloudEvent.GetSampledRate());
        }

        [Fact]
        public void SamplingJsonTranscode()
        {
            var jsonFormatter = new JsonEventFormatter();
            var cloudEvent1 = jsonFormatter.DecodeStructuredEvent(Encoding.UTF8.GetBytes(sampleJson));
            // Note that the value is just a string here, as we don't know the attribute type.
            Assert.Equal("1", cloudEvent1["sampledrate"]);

            var jsonData = jsonFormatter.EncodeStructuredEvent(cloudEvent1, out _);
            var cloudEvent = jsonFormatter.DecodeStructuredEvent(jsonData, Sampling.AllAttributes);

            // When parsing with the attributes in place, the value is propagated as an integer.
            Assert.Equal(1, cloudEvent["sampledrate"]);
            Assert.Equal(1, cloudEvent.GetSampledRate());
        }

        [Fact]
        public void SetAttributeValue_Invalid()
        {
            var cloudEvent = new CloudEvent(Sampling.AllAttributes);
            Assert.Throws<ArgumentException>(() => cloudEvent["sampledrate"] = 0);
        }

        [Fact]
        public void SetSampledRate()
        {
            var cloudEvent = new CloudEvent();
            cloudEvent.SetSampledRate(5);
            Assert.Equal(5, cloudEvent["sampledrate"]);

            cloudEvent.SetSampledRate(null);
            Assert.Null(cloudEvent["sampledrate"]);
        }

        [Fact]
        public void SetSampleRate_Invalid()
        {
            var cloudEvent = new CloudEvent();
            Assert.Throws<ArgumentException>(() => cloudEvent.SetSampledRate(0));
        }

        [Fact]
        public void GetSampledRate_NotSet()
        {
            var cloudEvent = new CloudEvent();
            Assert.Null(cloudEvent.GetSampledRate());
        }
    }
}