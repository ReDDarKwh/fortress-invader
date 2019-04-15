using System.Collections;
using System.Collections.Generic;
using TownGenerator.Geom;
using UnityEngine;
using System.Linq;
using TownGenerator.Wards;
using MoreLinq;
using TownGenerator.Building;

public class WardController : MonoBehaviour
{

    public PolygonCollider2D collider;
    public Ward ward;


    public GameObject followerGuardPrefab;
    public GameObject leaderGuardPrefab;


    private MeshRenderer meshRenderer;
    private MeshFilter filter;


    private CityController cityController;



    // Start is called before the first frame update
    void Start()
    {

        cityController = GetComponentInParent<CityController>();
        meshRenderer = GetComponent<MeshRenderer>();
        filter = GetComponent<MeshFilter>();
        setupCollider(collider, ward.patch.shape);
        setupMesh();

        // create Guard group formations
        /*
             g0 <-- leader
          g1   g2  <-- followers
          g3   g4       
         */


        // g0


        if (ward.patch.withinWalls)
        {

            // if (Random.value < 0.7)
            // {
            //     return;
            // }
            var accessablePoints = ward.patch.shape.Where(
                x =>
                {
                    return cityController.cityModel.topology.pt2node.ContainsKey(x);
                }
                );



            var startPoint = accessablePoints.First(x => !cityController.cityModel.wall.shape.Contains(x));

            Point furthestPointFromStart = accessablePoints.MaxBy(x => (x.vec - startPoint.vec).magnitude).FirstOrDefault();

            var leader = Instantiate(leaderGuardPrefab, transform.TransformPoint(startPoint.vec), Quaternion.identity).GetComponent<Leader>();

            leader.startToEndPath = cityController.cityModel.topology
            .buildPath(startPoint, furthestPointFromStart)
            .Select(x => transform.TransformPoint(x.vec)).ToList();

            leader.endToStartPath = cityController.cityModel.topology
            .buildPath(furthestPointFromStart, startPoint)
            .Select(x => transform.TransformPoint(x.vec)).ToList();

            leader.currentPath = leader.endToStartPath;

            //leader.path = cityController.cityModel.topology.buildPath(startPoint, path[path.Count - 1]).Select(x => transform.TransformPoint(x.vec)).ToList();
        }

        // g1
        // var follower = Instantiate(followerGuardPrefab).GetComponent<Follower>();
        // follower.leader = leader;
        // follower.offSetFromLeader = new Vector3(-1, -2);

        // // g2
        // follower = Instantiate(followerGuardPrefab).GetComponent<Follower>();
        // follower.leader = leader;
        // follower.offSetFromLeader = new Vector3(1, -2);

        // // g3
        // follower = Instantiate(followerGuardPrefab).GetComponent<Follower>();
        // follower.leader = leader;
        // follower.offSetFromLeader = new Vector3(-1, -4);

        // // g4
        // follower = Instantiate(followerGuardPrefab).GetComponent<Follower>();
        // follower.leader = leader;
        // follower.offSetFromLeader = new Vector3(1, -4);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void setupMesh()
    {
        // Create Vector2 vertices
        Vector2[] poly = ward.patch.shape.Select(x => x.vec).ToArray();
        int[] indices = new Triangulator(poly).Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[poly.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(poly[i].x, poly[i].y, 0);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        filter.mesh = msh;
    }

    private void setupCollider(PolygonCollider2D collider, Polygon corners)
    {
        collider.SetPath(0, corners.Concat(new List<Point> { corners[0] }).Select(x => x.vec).ToArray());
    }
}
