**Vizzy++** is a SimpleRockets 2 mod containing additional expression, instruction, and event blocks.

# Vizzy++ Elements

## Craft Information Expressions

* Orbital Element¹ ...
    * Right Ascension - The angle between the reference direction and the ascending node.
    * Inclination - The degree of tilt.
    * Argument of Periapsis - The angle between the ascending node and the direction of periapse.
    * True Anomaly - The angle between the direction of the periapse and the current position from the main focus of the orbital ellipse.
    * Semi-Major Axis - Half of the sum of the apoapse and periapse distances.
    * Eccentricity - The shape of the orbit, from circular (0), to parabolic (1), to hyperbolic (>1).
* Advanced Orbital Element¹ ...
    * Angular Momentum - A vector representing the body's rate of rotation around its parent.
    * Apoapse - A vector representing the apoapse of the orbit.
    * Periapse - A vector representing the periapse of the orbit.
    * Orbital Plane Normal - A unit vector perpendicular to the orbital plane.
    * Eccentricity Vector - A vector pointing toward the periapsis of the orbit with a magnitude equal to the eccentricity.
    * Eccentric Anomaly - The current eccentric anomaly. See: [https://en.wikipedia.org/wiki/Eccentric_anomaly](https://en.wikipedia.org/wiki/Eccentric_anomaly)
    * Mean Anomaly - The faction of an orbit's period that has elapsed since the orbit passed periapsis in degrees from 0 to 360.
    * Mean Motion - The average angular speed of the orbit in degrees.
* Cartesian State Vector¹ ...
    * Position - The position of the craft or planet relative to its parent.
    * Velocity - The velocity of the craft or planet.
* Target Craft ID - Craft names are sometimes duplicated, so this is more reliable for use with the other expressions.

¹ _These expressions can take a Planet Name, a Craft Name, or a Craft Id and return the specified value for that object's orbit. If the specified value is blank or a negative number, then the value for the current craft will be returned._

## Constant Expressions

* π - The mathematical constant PI, 3.14159...
* G - The Universal Gravitation Constant (in cubic meters per kilogram per seconds squared).
* _e_ - Euler's Number (base of natural logarithms).
* c - The speed of light in meters per second.

# Roadmap

 * String Expressions (Contains, Starts/Ends With, Regex Match)
 * Additional Current Craft Information
     * NavSphere settings: Pitch/Heading
     * Staging information
 * Camera Part Manipulation
     * Set the FOV of a camera part by id.
     * Set camera part orientation (in craft local coordinates)
     * Lock camera part orientation (prevent user movement with the mouse)
 * Prompt user for input (using a dialog, pauses the game)
 * Additional Craft (by Id) Information
     * Time of next closest approach
     * Distance at closest approach
 * Burn Node Manipulation
     * List burn nodes
     * Create a burn node at a given True Anomaly
     * Set burn node direction (in a vector relative to +X = prograde and +Y is the orbit normal)
     * Set burn node delta-v
     * Get orbital parameters after burn.
