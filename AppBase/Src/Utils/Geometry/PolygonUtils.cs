using NetTopologySuite.Geometries;

namespace AppBase.Utils.Geometry;

public class PolygonUtils
{
    private static int FindClosestEdgeIndex(Coordinate[] coordinates, Coordinate targetPoint)
    {
        int closestEdgeIndex = -1;
        double minDistance = double.MaxValue;

        // Iterate through all edges (from vertex i to vertex i+1)
        for (int i = 0; i < coordinates.Length - 1; i++)
        {
            double distance = CalculateDistanceToEdge(coordinates[i], coordinates[i + 1], targetPoint);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEdgeIndex = i;
            }
        }

        // Also check the edge from last vertex to first vertex (closing the polygon)
        double lastEdgeDistance =
            CalculateDistanceToEdge(coordinates[coordinates.Length - 1], coordinates[0], targetPoint);
        if (lastEdgeDistance < minDistance)
        {
            closestEdgeIndex = coordinates.Length - 1;
        }

        return closestEdgeIndex;
    }

    private static double CalculateDistanceToEdge(Coordinate start, Coordinate end, Coordinate point)
    {
        // Vector from start to end
        double edgeX = end.X - start.X;
        double edgeY = end.Y - start.Y;

        // Vector from start to point
        double pointX = point.X - start.X;
        double pointY = point.Y - start.Y;

        // Calculate dot product to find projection
        double dotProduct = pointX * edgeX + pointY * edgeY;
        double edgeLengthSquared = edgeX * edgeX + edgeY * edgeY;

        // If edge length is zero, return distance to start point
        if (edgeLengthSquared == 0)
            return Math.Sqrt(pointX * pointX + pointY * pointY);

        // Calculate projection parameter
        double t = dotProduct / edgeLengthSquared;

        // Clamp t to [0,1] to stay within the segment
        t = Math.Max(0, Math.Min(1, t));

        // Calculate closest point on the edge
        double closestX = start.X + t * edgeX;
        double closestY = start.Y + t * edgeY;

        // Calculate distance to closest point
        double dx = point.X - closestX;
        double dy = point.Y - closestY;

        return Math.Sqrt(dx * dx + dy * dy);
    }

    private static Coordinate[] InsertPointInEdge(Coordinate[] coordinates, int edgeIndex, Coordinate newPoint)
    {
        var newCoordinates = new List<Coordinate>();

        for (int i = 0; i < coordinates.Length; i++)
        {
            // Add the current vertex
            newCoordinates.Add(coordinates[i]);

            // If this is the edge where we need to insert the new point
            if (i == edgeIndex)
            {
                // Insert the new point after the current vertex
                newCoordinates.Add(newPoint);
            }
        }

        return newCoordinates.ToArray();
    }

    public static Polygon? AddPointToPolygonClosestEdge(Polygon polygon, Point point)
    {
        var newPoint = new Coordinate(point.X, point.Y);

        // Get exterior ring coordinates and handle polygon closure
        var exteriorRing = polygon.ExteriorRing;
        var coordinates = exteriorRing.Coordinates;

        // Check if polygon is closed (first and last points are the same)
        bool isClosed = coordinates.Length > 0 && coordinates[0].Equals2D(coordinates[coordinates.Length - 1]);
        var workingCoordinates = isClosed ? coordinates.Take(coordinates.Length - 1).ToArray() : coordinates;

        if (workingCoordinates.Length < 3)
        {
            throw new Exception("Cannot add point to a polygon with less than 3 coordinates");
        }

        // Find closest edge
        int closestEdgeIndex = FindClosestEdgeIndex(workingCoordinates, newPoint);

        // Insert new point
        var newExteriorCoordinates = InsertPointInEdge(workingCoordinates, closestEdgeIndex, newPoint);

        // Close the ring if it was originally closed
        if (isClosed)
        {
            newExteriorCoordinates =
                newExteriorCoordinates.Concat(new[] { newExteriorCoordinates[0].Copy() }).ToArray();
        }

        // Recreate polygon with exterior and interior rings
        var geometryFactory = new GeometryFactory();
        var newExteriorRing = geometryFactory.CreateLinearRing(newExteriorCoordinates);

        Polygon newPolygon;
        if (polygon.NumInteriorRings > 0)
        {
            var interiorRings = new LinearRing[polygon.NumInteriorRings];
            for (int i = 0; i < polygon.NumInteriorRings; i++)
            {
                interiorRings[i] = geometryFactory.CreateLinearRing(polygon.GetInteriorRingN(i).Coordinates);
            }

            newPolygon = geometryFactory.CreatePolygon(newExteriorRing, interiorRings);
        }
        else
        {
            newPolygon = geometryFactory.CreatePolygon(newExteriorRing);
        }

        // Validate the new polygon geometry
        if (!newPolygon.IsValid)
        {
            // return new BadRequestObjectResult("Resulting polygon geometry is invalid.");
            return null;
        }

        return newPolygon;
    }
}