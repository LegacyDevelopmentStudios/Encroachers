using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class EncTools
{
    /// <summary>
    /// Returns the angle in full 360 degrees.
    /// </summary>
    /// <param name="x" />
    /// <param name="y" />
    /// <returns>Returns a float. Uses Atan2.</returns>
	/// <example>
	/// <code>
	/// Mathf.Atan2(x, y)
	/// </code>
	/// </example>
    public static float AtanFULL(float x, float y)
    {
        float degrees = Mathf.Atan2(x, y) * (180f / Mathf.PI);
        if(degrees < 0f) { degrees += 360; }
        return degrees;
    }
    /// <summary>
    /// Returns the angle of a vector in full 360 degrees.
    /// </summary>
    /// <param name="v" />
    /// <returns>Returns a float. Uses Atan2.</returns>
	/// <example>
	/// <code>
	/// Mathf.Atan2(v.y, v.x)
	/// </code>
	/// </example>
    public static float AtanFULL(Vector2 v)
    {
        float degrees = Mathf.Atan2(v.y, v.x) * (180f / Mathf.PI);
        if(degrees < 0f) { degrees += 360; }
        return degrees;
    }




    /// <summary>
    /// Tests whether a value is within a range of minimum and maximum.
    /// </summary>
    /// <param name="_val" />
    /// <param name="_min" />
    /// <param name="_max" />
    /// <returns>Returns true if _val is inside the range and false if not.</returns>
    public static bool inRange(float _val, float _min, float _max)
    {
        if(_val < _min) { return false; }
        if(_val >_max) { return false; }
        return true;
    }




    /// <summary>
    /// Tests whether a given angle in degrees is within a given range based on a second angle.
    /// </summary>
    /// <param name="_angIN">The angle to test.</param>
    /// <param name="_angRange">The angle the range is based on.</param>
    /// <param name="_ccw">Counter-clockwise rotation from _angRange.</param>
    /// <param name="_cw">Clockwise rotation from _angRange.</param>
    /// <returns>Returns true if _angIN is inside the range and false if not.</returns>
    public static bool AngleInRange(float _angIN, float _angRange, float _ccw, float _cw)
    {
        bool oob = false;
        float min = _angRange - _ccw;
        if(min < 0f) { min += 360f; oob = true; }
        float max = _angRange + _cw;
        if(max > 360f) { max -= 360f; oob = true; }
        
        if (oob == true)
        {
            if(_angIN > min) { return true; }
            if(_angIN < max) { return true; }
        }
        else
        {
            if (_angIN > min && _angIN < max)
            {
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// Tests whether a given angle in degrees is within a given range based on a second angle.
    /// </summary>
    /// <param name="_angIN">The angle to test.</param>
    /// <param name="_angRange">The angle the range is based on.</param>
    /// <param name="_rng">Extends clockwise and counter-clockwise from _angRange.</param>
    /// <returns>Returns true if _angIN is inside the range and false if not.</returns>
    public static bool AngleInRange(float _angIN, float _angRange, float _rng)
    {
        return AngleInRange(_angIN, _angRange, _rng, _rng);
    }
    




    /// <summary>
    /// Returns the minimum difference between two angles in degrees.
    /// </summary>
    /// <param name="ang1" />
    /// <param name="ang2" />
    public static float MinAngDiff(float ang1, float ang2)
    {
        float left = ang1 - ang2;
        float right = ang2 - ang1;

        if(left < 0f) { left += 360f; }
        if(right < 0f) { right += 360f; }

        if (left > right)
        {
            return right;
        }
        else
        {
            return left;
        }
    }




    /// <summary>
    /// Applies a relative force to a rigid body for movement.
    /// </summary>
    /// <param name="_rb">The rigidbody the force applies to.</param>
    /// <param name="_force">A direction vector.(not normalized)</param>
    /// <param name="_forceMult">A scalar clamped between 0 and 1.</param>
    /// <param name="_scalar">A second scalar.</param>
    public static void ApplyMoveForce(ref Rigidbody _rb, Vector3 _force, float _forceMult, float _scalar)
    {
        
        _rb.AddRelativeForce(_force.normalized * _scalar * Mathf.Clamp(_forceMult, 0f, 1f), ForceMode.VelocityChange);
    }



    /// <summary>
    /// Negates the velocity of a rigid body and applies it to reduce velocity.
    /// </summary>
    /// <param name="_rb">The rigidbody the force applies to.</param>
    /// <param name="_dampMult">A scalar that controls overall dampening.</param>
    /// <param name="_dampHori">A scalar that controls dampening on the x and z axes.</param>
    /// <param name="_dampVert">A scalar that controls dampening on the y axis.</param>
    /// <param name="_scalar">A scalar clamped between 0 and 1.</param>
    public static void Dampen(ref Rigidbody _rb, float _dampMult, float _dampHori, float _dampVert, float _scalar)
    {
        if (_rb.velocity.magnitude > 0.05f || _scalar > 0f)
        {
            if (_dampMult > 0f)
            {
                if (_dampHori > 0f || _dampVert > 0f)
                {
                    Vector3 _vd;
                    _vd.x = -(_rb.velocity.x * _dampHori);
                    _vd.y = -(_rb.velocity.y * _dampVert);
                    _vd.z = -(_rb.velocity.z * _dampHori);
                    _rb.AddForce(_vd * _rb.mass * _dampMult * Mathf.Clamp(_scalar, 0f, 1f));
                }
            }
        }
    }





    /// <summary>
    /// Check if velocity y is greater than the sum of x and z.
    /// </summary>
    /// <param name="_rb" />
    public static bool CheckForFalling(Rigidbody _rb)
    {
        if (Mathf.Abs(_rb.velocity.y) > Mathf.Abs(_rb.velocity.x) + Mathf.Abs(_rb.velocity.z))
        {
            return true;
        }

        return false;
    }





    /// <summary>
    /// Apply a force in the upward direction. aka Jump.
    /// </summary>
    /// <param name="_rb">The rigidbody the force applies to.</param>
    /// <param name="tr">The transfrom of the game object.</param>
    /// <param name="force">Force in meters per second.</param>
    /// <param name="movement">Direction vector.(not normalized)</param>
    /// <remarks>Use GetJumpForce(float _meters_, float _gravity_) to get the force.</remarks>
    public static void Jump(ref Rigidbody rb, Transform tr, float force, Vector3 movement)
    {
        Vector3 newVec;
        newVec.x = movement.x;
        newVec.y = force;
        newVec.z = movement.z;
        newVec = tr.TransformDirection(newVec);
        rb.AddForce(newVec, ForceMode.Impulse);
    }



    // This returns a force in m/s to use in RB.AddForce(retVal, ForceMode.Impulse) to jump to a certain height in meters.
    /// <summary>
    /// This returns a force in meters per second to use to jump to a certain height in meters.
    /// </summary>
    /// <param name="_meters_">The height in meters.</param>
    /// <param name="_gravity_">Physics.gravity.y</param>
    public static float GetJumpForce(float _meters_, float _gravity_)
    {
        return Mathf.Sqrt(-(2f * _gravity_ * _meters_));
    }




    /// <summary>
    /// Detect collision with ground/object
    /// </summary>
    /// <param name="origin">Position to cast from.</param>
    /// <param name="collider">The collider to check for collision.</param>
    /// <param name="maxDistance">The distance of the cast.</param>
    /// <param name="hit">The variable to output the hit info to.</param>
    public static bool IsGrounded(Vector3 origin, Collider collider, float maxDistance, out RaycastHit hit )
    {
        return Physics.SphereCast(origin, collider.bounds.extents.x,  -Vector3.up,  out hit, maxDistance);
    }




    /// <summary>Toggle the cursor visibility.</summary>
     public static void ToggleCursorState()
    {
        Cursor.lockState = (Cursor.visible) ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !Cursor.visible;
    }
    
}
