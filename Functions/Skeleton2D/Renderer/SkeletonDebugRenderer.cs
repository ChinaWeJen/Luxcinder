using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.Skeleton2D.Renderer.DrawCommands;
using Microsoft.Xna.Framework;
using Spine;

namespace Luxcinder.Functions.Skeleton2D.Renderer;

public class SkeletonDebugRenderer
{
	private GeometryBuffer renderer;

	public static Color boneLineColor = new Color(1f, 0f, 0f, 1f);
	public static Color boneOriginColor = new Color(0f, 1f, 0f, 1f);
	public static Color attachmentLineColor = new Color(0f, 0f, 1f, 0.5f);
	public static Color triangleLineColor = new Color(1f, 0.64f, 0f, 0.5f);
	public static Color pathColor = new Color(1f, 0.5f, 0f, 1f);
	public static Color clipColor = new Color(0.8f, 0f, 0f, 1f);
	public static Color clipDecomposedColor = new Color(0.8f, 0.8f, 0f, 1f);
	public static Color aabbColor = new Color(0f, 1f, 0f, 0.5f);

	/// <summary>A Z position offset added at each vertex.</summary>
	private float z = 0.0f;

	public float Z
	{
		get { return z; }
		set { z = value; }
	}

	public bool DrawBones { get; set; }

	public bool DrawRegionAttachments { get; set; }

	public bool DrawBoundingBoxes { get; set; }

	public bool DrawMeshHull { get; set; }

	public bool DrawMeshTriangles { get; set; }

	public bool DrawPaths { get; set; }

	public bool DrawClipping { get; set; }

	public bool DrawClippingDecomposed { get; set; }

	public bool DrawSkeletonXY { get; set; }

	public void DisableAll()
	{
		DrawBones = false;
		DrawRegionAttachments = false;
		DrawBoundingBoxes = false;
		DrawMeshHull = false;
		DrawMeshTriangles = false;
		DrawPaths = false;
		DrawClipping = false;
		DrawSkeletonXY = false;
	}

	public void EnableAll()
	{
		DrawBones = true;
		DrawRegionAttachments = true;
		DrawBoundingBoxes = true;
		DrawMeshHull = true;
		DrawMeshTriangles = true;
		DrawPaths = true;
		DrawClipping = true;
		DrawSkeletonXY = true;
	}

	private float[] vertices = new float[1024 * 2];
	private SkeletonBounds bounds = new SkeletonBounds();
	private Triangulator triangulator = new Triangulator();

	public SkeletonDebugRenderer()
	{
		renderer = new GeometryBuffer();
		EnableAll();
	}

	public DrawCommandList Draw(Skeleton2D skeleton2d)
	{
		var skeleton = skeleton2d.Skeleton;
		var skeletonX = skeleton.X;
		var skeletonY = skeleton.Y;

		renderer.Begin();

		var bones = skeleton.Bones;
		if (DrawBones)
		{
			renderer.SetColor(boneLineColor);
			for (int i = 0, n = bones.Count; i < n; i++)
			{
				var bone = bones.Items[i];
				if (bone.Parent == null)
					continue;
				var x = bone.Data.Length * bone.A + bone.WorldX;
				var y = bone.Data.Length * bone.C + bone.WorldY;
				renderer.Line(bone.WorldX, bone.WorldY, x, y);
			}
			if (DrawSkeletonXY)
			{
				renderer.X(skeletonX, skeletonY, 4);
			}
		}

		if (DrawRegionAttachments)
		{
			renderer.SetColor(attachmentLineColor);
			var slots = skeleton.Slots;
			for (int i = 0, n = slots.Count; i < n; i++)
			{
				var slot = slots.Items[i];
				var attachment = slot.Attachment;
				if (attachment is RegionAttachment)
				{
					var regionAttachment = (RegionAttachment)attachment;
					var vertices = this.vertices;
					regionAttachment.ComputeWorldVertices(slot.Bone, vertices, 0, 2);
					renderer.Line(vertices[0], vertices[1], vertices[2], vertices[3]);
					renderer.Line(vertices[2], vertices[3], vertices[4], vertices[5]);
					renderer.Line(vertices[4], vertices[5], vertices[6], vertices[7]);
					renderer.Line(vertices[6], vertices[7], vertices[0], vertices[1]);
				}
			}
		}

		if (DrawMeshHull || DrawMeshTriangles)
		{
			var slots = skeleton.Slots;
			for (int i = 0, n = slots.Count; i < n; i++)
			{
				var slot = slots.Items[i];
				var attachment = slot.Attachment;
				if (!(attachment is MeshAttachment))
					continue;
				var mesh = (MeshAttachment)attachment;
				var world = vertices = vertices.Length < mesh.WorldVerticesLength ? new float[mesh.WorldVerticesLength] : vertices;
				mesh.ComputeWorldVertices(slot, 0, mesh.WorldVerticesLength, world, 0, 2);
				int[] triangles = mesh.Triangles;
				var hullLength = mesh.HullLength;
				if (DrawMeshTriangles)
				{
					renderer.SetColor(triangleLineColor);
					for (int ii = 0, nn = triangles.Count(); ii < nn; ii += 3)
					{
						int v1 = triangles[ii] * 2, v2 = triangles[ii + 1] * 2, v3 = triangles[ii + 2] * 2;
						renderer.Triangle(world[v1], world[v1 + 1], //
							world[v2], world[v2 + 1], //
							world[v3], world[v3 + 1] //
						);
					}
				}
				if (DrawMeshHull && hullLength > 0)
				{
					renderer.SetColor(attachmentLineColor);
					hullLength = (hullLength >> 1) * 2;
					float lastX = vertices[hullLength - 2], lastY = vertices[hullLength - 1];
					for (int ii = 0, nn = hullLength; ii < nn; ii += 2)
					{
						float x = vertices[ii], y = vertices[ii + 1];
						renderer.Line(x, y, lastX, lastY);
						lastX = x;
						lastY = y;
					}
				}
			}
		}

		if (DrawBoundingBoxes)
		{
			var bounds = this.bounds;
			bounds.Update(skeleton, true);
			renderer.SetColor(aabbColor);
			renderer.Rect(bounds.MinX, bounds.MinY, bounds.Width, bounds.Height);
			var polygons = bounds.Polygons;
			var boxes = bounds.BoundingBoxes;
			for (int i = 0, n = polygons.Count; i < n; i++)
			{
				var polygon = polygons.Items[i];
				renderer.Polygon(polygon.Vertices, 0, polygon.Count);
			}
		}

		if (DrawBones)
		{
			renderer.SetColor(boneOriginColor);
			for (int i = 0, n = bones.Count; i < n; i++)
			{
				var bone = bones.Items[i];
				renderer.Circle(bone.WorldX, bone.WorldY, 3);
			}
		}

		if (DrawClipping)
		{
			var slots = skeleton.Slots;
			renderer.SetColor(clipColor);
			for (int i = 0, n = slots.Count; i < n; i++)
			{
				var slot = slots.Items[i];
				var attachment = slot.Attachment;
				if (!(attachment is ClippingAttachment))
					continue;
				var clip = (ClippingAttachment)attachment;
				var nn = clip.WorldVerticesLength;
				var world = vertices = vertices.Length < nn ? new float[nn] : vertices;
				clip.ComputeWorldVertices(slot, 0, nn, world, 0, 2);
				ExposedList<float> clippingPolygon = new ExposedList<float>();
				for (int ii = 0; ii < nn; ii += 2)
				{
					var x = world[ii];
					var y = world[ii + 1];
					var x2 = world[(ii + 2) % nn];
					var y2 = world[(ii + 3) % nn];
					renderer.Line(x, y, x2, y2);
					clippingPolygon.Add(x);
					clippingPolygon.Add(y);
				}

				if (DrawClippingDecomposed)
				{
					SkeletonClipping.MakeClockwise(clippingPolygon);
					var triangles = triangulator.Triangulate(clippingPolygon);
					var clippingPolygons = triangulator.Decompose(clippingPolygon, triangles);
					renderer.SetColor(clipDecomposedColor);
					foreach (var polygon in clippingPolygons)
					{
						SkeletonClipping.MakeClockwise(polygon);
						polygon.Add(polygon.Items[0]);
						polygon.Add(polygon.Items[1]);
						renderer.Polygon(polygon.Items, 0, polygon.Count >> 1);
					}
				}
			}
		}
		return renderer.End();
	}
}