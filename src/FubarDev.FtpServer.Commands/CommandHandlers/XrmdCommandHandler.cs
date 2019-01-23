//-----------------------------------------------------------------------
// <copyright file="XrmdCommandHandler.cs" company="Fubar Development Junker">
//     Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>
// <author>Mark Junker</author>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

using FubarDev.FtpServer.FileSystem;

namespace FubarDev.FtpServer.CommandHandlers
{
    /// <summary>
    /// Implements the <c>XRMD</c> command.
    /// </summary>
    public class XrmdCommandHandler : FtpCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XrmdCommandHandler"/> class.
        /// </summary>
        /// <param name="connectionAccessor">The accessor to get the connection that is active during the <see cref="Process"/> method execution.</param>
        public XrmdCommandHandler(IFtpConnectionAccessor connectionAccessor)
            : base(connectionAccessor, "XRMD")
        {
        }

        /// <inheritdoc/>
        public override async Task<FtpResponse> Process(FtpCommand command, CancellationToken cancellationToken)
        {
            var path = command.Argument;
            var currentPath = Data.Path.Clone();
            var subDir = await Data.FileSystem.GetDirectoryAsync(currentPath, path, cancellationToken).ConfigureAwait(false);
            if (subDir == null)
            {
                return new FtpResponse(550, "Not a valid directory.");
            }

            try
            {
                if (Data.Path.IsChildOfOrSameAs(currentPath, Data.FileSystem))
                {
                    return new FtpResponse(550, "Not a valid directory (is same or parent of current directory).");
                }

                await Data.FileSystem.UnlinkAsync(subDir, cancellationToken).ConfigureAwait(false);
                return new FtpResponse(250, "Directory removed.");
            }
            catch (Exception)
            {
                return new FtpResponse(521, "Couldn't remove directory (locked?).");
            }
        }
    }
}
