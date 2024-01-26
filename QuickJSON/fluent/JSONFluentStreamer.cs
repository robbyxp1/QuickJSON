/*
 * Copyright 2024-2024 Robbyxp1 @ github.com
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 */

using System;
using System.IO;

namespace QuickJSON
{
    /// <summary>
    /// Quick formatter using Fluent syntax, quick and easy way to make a JSON string
    /// </summary>

    public class JSONFormatterStreamer : JSONFormatter, IDisposable
    {
        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="stream">stream to write to</param>
        /// <param name="writeblock">length of data before write</param>
        /// <param name="encoding">character encoding (default UTF8)</param>
        public JSONFormatterStreamer(Stream stream, int writeblock = 1000, System.Text.Encoding encoding = null ) : base()
        {
            enc = encoding != null ? encoding : new System.Text.UTF8Encoding();

            this.stream = stream;
            this.writeblock = writeblock;
        }

        /// <summary>
        /// Internal prefix function
        /// </summary>
        /// <param name="named">is named parameter</param>
        protected override void Prefix(bool named)
        {
            if ( Length >= writeblock )
            {
                var bytes = enc.GetBytes(CurrentText);
                stream.Write(bytes, 0, bytes.Length);
               // System.Diagnostics.Debug.Write($"Formatter Wrote block");
                Clear();
            }

            base.Prefix(named);
        }

        /// <summary>
        /// Close() and then write all of the data to the stream
        /// </summary>
        public void Dispose()
        {
            Close();
            if ( Length >= 0 )
            {
                var bytes = enc.GetBytes(CurrentText);
                stream.Write(bytes, 0, bytes.Length);
                Clear();
            }
        }

        private System.Text.Encoding enc;
        private Stream stream;
        private int writeblock;
    }
}
