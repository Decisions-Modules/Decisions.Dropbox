using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Dropbox.Api.Sharing;

namespace Decisions.DropboxApi
{
    [DataContract]
    public class DropboxFileMeta
    {
        /// <summary>
        /// <para>The ID of the file.</para>
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// <para>The name of this file.</para>
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// <para>Policies governing this shared file.</para>
        /// </summary>
        [DataMember]
        public FolderPolicy Policy { get; set; }

        /// <summary>
        /// <para>URL for displaying a web preview of the shared file.</para>
        /// </summary>
        [DataMember]
        public string PreviewUrl { get; set; }

        /// <summary>
        /// <para>The current user's access level for this shared file.</para>
        /// </summary>
        [DataMember]
        public DropBoxAccessLevel AccessType { get; set; }

        /// <summary>
        /// <para>The expected metadata of the link associated for the file when it is first
        /// shared. Absent if the link already exists. This is for an unreleased feature so it
        /// may not be returned yet.</para>
        /// </summary>
        [DataMember]
        public ExpectedSharedContentLinkMetadata ExpectedLinkMetadata { get; set; }

        /// <summary>
        /// <para>The metadata of the link associated for the file. This is for an unreleased
        /// feature so it may not be returned yet.</para>
        /// </summary>
        [DataMember]
        public SharedContentLinkMetadata LinkMetadata { get; set; }

        /// <summary>
        /// <para>The display names of the users that own the file. If the file is part of a
        /// team folder, the display names of the team admins are also included. Absent if the
        /// owner display names cannot be fetched.</para>
        /// </summary>
        [DataMember]
        public string[] OwnerDisplayNames { get; set; }

        /// <summary>
        /// <para>The team that owns the file. This field is not present if the file is not
        /// owned by a team.</para>
        /// </summary>
        [DataMember]
        public string OwnerTeamId { get; set; }

        [DataMember]
        public string OwnerTeamName { get; set; }

        /// <summary>
        /// <para>The ID of the parent shared folder. This field is present only if the file is
        /// contained within a shared folder.</para>
        /// </summary>
        [DataMember]
        public string ParentSharedFolderId { get; set; }

        /// <summary>
        /// <para>The cased path to be used for display purposes only. In rare instances the
        /// casing will not correctly match the user's filesystem, but this behavior will match
        /// the path provided in the Core API v1. Absent for unmounted files.</para>
        /// </summary>
        [DataMember]
        public string PathDisplay { get; set; }

        /// <summary>
        /// <para>The lower-case full path of this file. Absent for unmounted files.</para>
        /// </summary>
        [DataMember]
        public string PathLower { get; set; }

        /// <summary>
        /// <para>The sharing permissions that requesting user has on this file. This
        /// corresponds to the entries given in <see cref="P:Dropbox.Api.Sharing.GetFileMetadataBatchArg.Actions" /> or <see cref="P:Dropbox.Api.Sharing.GetFileMetadataArg.Actions" />.</para>
        /// </summary>
        [DataMember]
        public FilePermission[] Permissions { get; set; }

        /// <summary>
        /// <para>Timestamp indicating when the current user was invited to this shared file.
        /// If the user was not invited to the shared file, the timestamp will indicate when
        /// the user was invited to the parent shared folder. This value may be absent.</para>
        /// </summary>
        [DataMember]
        public DateTime? TimeInvited { get; set; }
    }

   /* FolderPolicy
    DropBoxAccessLevel    AccessLevel
        ExpectedSharedContentLinkMetadata
        SharedContentLinkMetadata
        FilePermission*/

}