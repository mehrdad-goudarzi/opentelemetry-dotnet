// <copyright file="BatchMetricPoint.cs" company="OpenTelemetry Authors">
// Copyright The OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenTelemetry.Metrics
{
    public readonly struct BatchMetricPoint : IDisposable
    {
        private readonly MetricPoint[] metricsPoints;
        private readonly long targetCount;
        private readonly DateTimeOffset start;
        private readonly DateTimeOffset end;

        internal BatchMetricPoint(MetricPoint[] metricsPoints, int maxSize, DateTimeOffset start, DateTimeOffset end)
        {
            Debug.Assert(maxSize > 0, $"{nameof(maxSize)} should be a positive number.");
            this.metricsPoints = metricsPoints ?? throw new ArgumentNullException(nameof(metricsPoints));
            this.targetCount = maxSize;
            this.start = start;
            this.end = end;
        }

        /// <inheritdoc/>
        public void Dispose()
        {

        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Batch{T}"/>.
        /// </summary>
        /// <returns><see cref="Enumerator"/>.</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this.metricsPoints, this.targetCount, this.start, this.end);
        }

        /// <summary>
        /// Enumerates the elements of a <see cref="Batch{T}"/>.
        /// </summary>
        public struct Enumerator : IEnumerator
        {
            private readonly MetricPoint[] metricsPoints;
            private readonly DateTimeOffset start;
            private readonly DateTimeOffset end;
            private long targetCount;
            private long index;

            internal Enumerator(MetricPoint[] metricsPoints, long targetCount, DateTimeOffset start, DateTimeOffset end)
            {
                this.Current = default;
                this.metricsPoints = metricsPoints;
                this.targetCount = targetCount;
                this.index = 0;
                this.start = start;
                this.end = end;
            }

            /// <inheritdoc/>
            public MetricPoint Current { get; private set; }

            /// <inheritdoc/>
            object IEnumerator.Current => this.Current;

            /// <inheritdoc/>
            public void Dispose()
            {
            }

            /// <inheritdoc/>
            public bool MoveNext()
            {
                var metricPoints = this.metricsPoints;
                if (this.index < this.targetCount)
                {
                    metricPoints[this.index].StartTime = this.start;
                    metricPoints[this.index].EndTime = this.end;
                    this.Current = metricPoints[this.index];
                    this.index++;
                    return true;
                }

                this.Current = default;
                return false;
            }

            /// <inheritdoc/>
            public void Reset()
                => throw new NotSupportedException();
        }
    }
}