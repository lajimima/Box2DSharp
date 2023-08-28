using System.Diagnostics;
using System.Runtime.CompilerServices;
using FixedBox2D.Collision;
using FixedBox2D.Collision.Collider;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;

namespace FixedBox2D.Dynamics.Contacts
{
    /// <summary>
    ///     边缘与多边形接触
    /// </summary>
    public class EdgeAndPolygonContact : Contact
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void Evaluate(ref Manifold manifold, in Transform xfA, Transform xfB)
        {
            CollisionUtils.CollideEdgeAndPolygon(
                ref manifold,
                (EdgeShape)FixtureA.Shape,
                xfA,
                (PolygonShape)FixtureB.Shape,
                xfB);
        }
    }

    internal class EdgeAndPolygonContactFactory : IContactFactory
    {
        private readonly ContactPool<EdgeAndPolygonContact> _pool = new ContactPool<EdgeAndPolygonContact>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Contact Create(Fixture fixtureA, int indexA, Fixture fixtureB, int indexB)
        {
            Debug.Assert(fixtureA.ShapeType == ShapeType.Edge);
            Debug.Assert(fixtureB.ShapeType == ShapeType.Polygon);
            var contact = _pool.Get();
            contact.Initialize(fixtureA, 0, fixtureB, 0);
            return contact;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy(Contact contact)
        {
            _pool.Return((EdgeAndPolygonContact)contact);
        }
    }
}