﻿using MediaBrowser.Controller;
using MediaBrowser.Model.Dlna;
using ServiceStack;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaBrowser.Api.Playback.Hls
{
    /// <summary>
    /// Class GetHlsAudioSegment
    /// </summary>
    [Route("/Audio/{Id}/hls/{SegmentId}/stream.mp3", "GET")]
    [Route("/Audio/{Id}/hls/{SegmentId}/stream.aac", "GET")]
    [Api(Description = "Gets an Http live streaming segment file. Internal use only.")]
    public class GetHlsAudioSegment
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the segment id.
        /// </summary>
        /// <value>The segment id.</value>
        public string SegmentId { get; set; }
    }

    /// <summary>
    /// Class GetHlsVideoSegment
    /// </summary>
    [Route("/Videos/{Id}/hls/{PlaylistId}/stream.m3u8", "GET")]
    [Api(Description = "Gets an Http live streaming segment file. Internal use only.")]
    public class GetHlsPlaylist
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }

        public string PlaylistId { get; set; }
    }

    [Route("/Videos/ActiveEncodings", "DELETE")]
    [Api(Description = "Stops an encoding process")]
    public class StopEncodingProcess
    {
        [ApiMember(Name = "DeviceId", Description = "The device id of the client requesting. Used to stop encoding processes when needed.", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "DELETE")]
        public string DeviceId { get; set; }
    }

    public class HlsSegmentService : BaseApiService
    {
        private readonly IServerApplicationPaths _appPaths;

        public HlsSegmentService(IServerApplicationPaths appPaths)
        {
            _appPaths = appPaths;
        }

        public object Get(GetHlsPlaylist request)
        {
            var file = request.PlaylistId + Path.GetExtension(Request.PathInfo);

            file = Path.Combine(_appPaths.TranscodingTempPath, EncodingContext.Streaming.ToString().ToLower(), file);

            return ResultFactory.GetStaticFileResult(Request, file, FileShare.ReadWrite);
        }

        public void Delete(StopEncodingProcess request)
        {
            ApiEntryPoint.Instance.KillTranscodingJobs(request.DeviceId, path => true);
        }

        /// <summary>
        /// Gets the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>System.Object.</returns>
        public object Get(GetHlsAudioSegment request)
        {
            var file = request.SegmentId + Path.GetExtension(Request.PathInfo);

            file = Path.Combine(_appPaths.TranscodingTempPath, EncodingContext.Streaming.ToString().ToLower(), file);

            return ResultFactory.GetStaticFileResult(Request, file, FileShare.ReadWrite);
        }
    }
}
