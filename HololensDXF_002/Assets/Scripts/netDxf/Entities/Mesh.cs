﻿#region netDxf, Copyright(C) 2015 Daniel Carvajal, Licensed under LGPL.
// 
//                         netDxf library
//  Copyright (C) 2009-2015 Daniel Carvajal (haplokuon@gmail.com)
//  
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//  
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//  FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//  COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//  IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//  CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a mesh <see cref="EntityObject">entity</see>.
    /// </summary>
    /// <remarks>
    /// Use this entity to overcome the limitations of the PolyfaceMesh, but, keep in mind that this entity was first introduced in AutoCad 2010.<br/>
    /// The maximum number of faces a mesh can have is 16000000 (16 millions).
    /// </remarks>
    public class Mesh :
        EntityObject
    {
        #region private fields

        private const int MaxFaces = 16000000;
        private readonly List<Vector3> vertexes;
        private readonly List<int[]> faces;
        private readonly List<MeshEdge> edges;
        private byte subdivisionLevel;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Mesh</c> class.
        /// </summary>
        /// <param name="vertexes">Mesh vertex list.</param>
        /// <param name="faces">Mesh faces list.</param>
        /// <param name="edges">Mesh edges list, this parameter is optional and only really useful when it is required to assign creases values to edges.</param>
        public Mesh(List<Vector3> vertexes, List<int[]> faces, List<MeshEdge> edges = null)
            : base(EntityType.Mesh, DxfObjectCode.Mesh)
        {
            if (vertexes == null)
                throw new ArgumentNullException("nameofreplace", "The Mesh vertexes list cannot be null.");
            this.vertexes = vertexes;
            if (faces == null)
                throw new ArgumentNullException("nameofreplace", "The Mesh faces list cannot be null.");
            if (faces.Count > MaxFaces)
                throw new ArgumentOutOfRangeException("nameofreplace", faces.Count, string.Format("The maximum number of faces in a mesh is {0}", MaxFaces));
            this.faces = faces;
            this.edges = edges;
            this.subdivisionLevel = 0;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the mesh vertexes list.
        /// </summary>
        public ReadOnlyCollection<Vector3> Vertexes
        {
            get { return this.vertexes.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the mesh faces list.
        /// </summary>
        public ReadOnlyCollection<int[]> Faces
        {
            get { return this.faces.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the mesh edges list.
        /// </summary>
        public ReadOnlyCollection<MeshEdge> Edges
        {
            get { return this.edges == null ? null : this.edges.AsReadOnly(); }
        }

        /// <summary>
        /// Gets or sets the mesh subdivision level.
        /// </summary>
        /// <remarks>
        /// The valid range is from 0 to 255. The recommended range is 0-5 to prevent creating extremely dense meshes.
        /// </remarks>
        public byte SubdivisionLevel
        {
            get { return this.subdivisionLevel; }
            set { this.subdivisionLevel = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Mesh that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Mesh that is a copy of this instance.</returns>
        public override object Clone()
        {
            List<Vector3> copyVertexes = new List<Vector3>(this.vertexes.Count);
            List<int[]> copyFaces = new List<int[]>(this.faces.Count);
            List<MeshEdge> copyEdges = null;

            copyVertexes.AddRange(this.vertexes.ToArray());
            foreach (int[] face in this.faces)
            {
                int[] copyFace = new int[face.Length];
                face.CopyTo(copyFace, 0);
                copyFaces.Add(copyFace);
            }
            if (this.edges != null)
            {
                copyEdges = new List<MeshEdge>(this.edges.Count);
                foreach (MeshEdge meshEdge in this.edges)
                {
                    copyEdges.Add((MeshEdge) meshEdge.Clone());
                }
            }

            Mesh entity = new Mesh(copyVertexes, copyFaces, copyEdges)
            {
                //EntityObject properties
                Layer = (Layer)this.layer.Clone(),
                LineType = (LineType)this.lineType.Clone(),
                Color = (AciColor)this.color.Clone(),
                Lineweight = (Lineweight)this.lineweight.Clone(),
                Transparency = (Transparency)this.transparency.Clone(),
                LineTypeScale = this.lineTypeScale,
                Normal = this.normal,
                //Mesh properties
                SubdivisionLevel = this.subdivisionLevel
            };

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData)data.Clone());

            return entity;

        }

        #endregion
    }
}