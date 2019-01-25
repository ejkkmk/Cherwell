using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GeometricLayouts.Models;

namespace GeometricLayouts.Controllers
{
    public class CoordinatesController : ApiController
    {
        // Define constants for values in the event our row and col ranges change or the triangle side lengths change in the future
        private const int _SideLength = 10;
        private const char _MinRow = 'A';
        private const char _MaxRow = 'F';
        private const int _MinCol = 1;
        private const int _MaxCol = 12;

        Coordinate coordinates = new Coordinate();


        // This method will return the coordinates for the given Row and Column values
        public IHttpActionResult GetCoordinatesRowCol(string rowString, string colString)
        {
            CalculateCoordinates(rowString, colString);
            return Ok(coordinates);

        }


        // This method will determine the Row and Column values based on a set of 3 points of a triangle.
        public IHttpActionResult GetRowCol(int pt1x, int pt1y, int pt2x, int pt2y, int pt3x, int pt3y)
        {
            int RowOffset;  // Used to determine the row offset for calculating the Y coordinate
            int ColOffset;  // Used to determine the col offset for calculating the X coordinate

            char Row;       // The calculated Row value based on the given coordinates
            int Col;        // The calculated Column value based on the given coordinates

            bool Verified = false;        // Flag used to determine if the calculated Row and Col values are valid
            RowCol rowcol = new RowCol(); // Instance of the data model we will be returning

            try
            {
                // This next section of code will determine the minimum y value for the given points, basically the upper most y value
                if (pt1y <= pt2y)
                {
                    if (pt1y <= pt3y)
                    {
                        RowOffset = pt1y;
                    }

                    else
                    {
                        RowOffset = pt3y;
                    }
                }

                else if (pt2y <= pt3y)
                {
                    RowOffset = pt2y;
                }

                else
                {
                    RowOffset = pt3y;
                }

                // Determine the Row based on our RowOffset value (which is based on the upper most y value)
                Row = Convert.ToChar(_MinRow + (RowOffset / _SideLength));

                // This next section will determine the minimum x value for the given points so we can determine the left most x value
                if (pt1x <= pt2x)
                {
                    if (pt1x <= pt3x)
                    {
                        ColOffset = pt1x;
                    }

                    else
                    {
                        ColOffset = pt3x;
                    }
                }

                else if (pt2x <= pt3x)
                {
                    ColOffset = pt2x;
                }

                else
                {
                    ColOffset = pt3x;
                }

                // Calculate the initial Col value for the left most triangle in the column.
                Col = ColOffset / _SideLength * 2 + 1;

                // Because the columns are divided diagonaly we need to determine which of the 2 triangles we are associated with
                // the left most or the right most.  This if section will determine if we are in the right most triangle and if so,
                // we need to add one to our Col value. 
                if (pt1x == pt2x)
                {
                    if (pt1x > pt3x)
                        Col++;
                }

                else if (pt1x == pt3x)
                {
                    if (pt1x > pt2x)
                        Col++;
                }

                else if (pt2x == pt3x)
                {
                    if (pt2x > pt1x)
                        Col++;
                }

                // The row and col values have been calculated, but because each triangle has a unique set of points, 
                // we can use the calculated Row and Col values to input them into the CalculatedCoordinates function to verify 
                // the returned points from the function match the give points.
                if (CalculateCoordinates(Row.ToString(), Col.ToString()))
                {
                    //The function successfully returned a set of points based on the Row and Col values we provided.  
                    // With the calculated points from the function and the points provided in our API call, make sure the 
                    // two set of triangle points match exactly. Verify all points in the two given sets match.
                    if ((coordinates.V1x == pt1x) && (coordinates.V1y == pt1y))
                    {
                        if ((coordinates.V2x == pt2x) && (coordinates.V2y == pt2y) && (coordinates.V3x == pt3x) && (coordinates.V3y == pt3y))
                        {
                            Verified = true;
                        }

                        else if ((coordinates.V2x == pt3x) && (coordinates.V2y == pt3y) && (coordinates.V3x == pt2x) && (coordinates.V3y == pt2y))
                        {
                            Verified = true;
                        }
                    }

                    if ((coordinates.V2x == pt1x) && (coordinates.V2y == pt1y))
                    {
                        if ((coordinates.V1x == pt2x) && (coordinates.V1y == pt2y) && (coordinates.V3x == pt3x) && (coordinates.V3y == pt3y))
                        {
                            Verified = true;
                        }

                        else if ((coordinates.V1x == pt3x) && (coordinates.V1y == pt3y) && (coordinates.V3x == pt2x) && (coordinates.V3y == pt2y))
                        {
                            Verified = true;
                        }
                    }

                    if ((coordinates.V3x == pt1x) && (coordinates.V3y == pt1y))
                    {
                        if ((coordinates.V1x == pt2x) && (coordinates.V1y == pt2y) && (coordinates.V2x == pt3x) && (coordinates.V2y == pt3y))
                        {
                            Verified = true;
                        }

                        else if ((coordinates.V1x == pt3x) && (coordinates.V1y == pt3y) && (coordinates.V2x == pt2x) && (coordinates.V2y == pt2y))
                        {
                            Verified = true;
                        }
                    }

                    // The calculated points have been verifed, return the values
                    if (Verified)
                    {
                        rowcol.Row = Row.ToString();
                        rowcol.Col = Col.ToString();
                        rowcol.Status = 0;
                    }

                    // The calculated points do not match the points returned by the function, return an error Status value
                    else
                    {
                        rowcol.Row = "";
                        rowcol.Col = "";
                        rowcol.Status = -1;
                    }
                }
            }

            catch
            {
                rowcol.Row = "";
                rowcol.Col = "";
                rowcol.Status = -1;
            }

            return Ok(rowcol);
        }


        // This function will calculate the triangles points based on a given row and col value.  If the
        // points were successfully calculated then return true, otherwise false.
        public bool CalculateCoordinates(string rowString, string colString)
        {
            bool Result = false;

            int ColNum;     
            char RowNum;

            int xOffset;    // The x value of the upper-left point of the triangle
            int yOffset;    // The y value of the upper-left point of the triangle

            coordinates.Status = -1;
            coordinates.V1x = 0;
            coordinates.V1y = 0;

            coordinates.V2x = 0;
            coordinates.V2y = 0;

            coordinates.V3x = 0;
            coordinates.V3y = 0;

            try
            {
                RowNum = Convert.ToChar(rowString.ToUpper());
                ColNum = Convert.ToInt32(colString);

                // Verify the row and column values are in the expected ranges
                if ((RowNum >= _MinRow) && (RowNum <= _MaxRow) && (ColNum >= _MinCol) && (ColNum <= _MaxCol))
                {
                    // The calculated xOffset and yOffset will be the x and y coordinates for the upper-left most point of the triangle
                    // Determine the yOffset based on the given row letter based on the _MinRow being the starting letter.
                    yOffset = (RowNum - _MinRow) * _SideLength;

                    // If the modulus of the given column number by 2 is 0 then we are in the "right most"
                    // triangle in the column
                    if (ColNum % 2 == 0)
                        xOffset = (ColNum / 2 - 1) * _SideLength;

                    // We are in the "left most" triangle in the column
                    else
                        xOffset = ((ColNum - 1) / 2) * _SideLength;

                    // Now that we have the x and y coordinates for the upper-left most point of the triangle
                    // we can calculated the other points based on the _SideLength value

                    // We are in the "left most" triangle...
                    if (ColNum % 2 == 1)
                    {
                        coordinates.V1x = xOffset + 1;
                        coordinates.V1y = yOffset + 1;

                        coordinates.V2x = coordinates.V1x;
                        coordinates.V2y = coordinates.V1y + _SideLength - 1;

                        coordinates.V3x = coordinates.V2x + _SideLength - 1;
                        coordinates.V3y = coordinates.V2y;
                    }

                    // We are in the "right most" triangle...
                    else
                    {
                        coordinates.V1x = xOffset + 1;
                        coordinates.V1y = yOffset + 1;

                        coordinates.V2x = coordinates.V1x + _SideLength - 1;
                        coordinates.V2y = coordinates.V1y;

                        coordinates.V3x = coordinates.V2x;
                        coordinates.V3y = coordinates.V2y + _SideLength - 1;
                    }

                    coordinates.Status = 0;
                    Result = true;
                }

                // The given row and column are outside our range(s) return an error status code
                else
                {
                    coordinates.Status = -1;
                }
 
            }

            catch
            {
                coordinates.Status = -1;
            }
            
            return Result;
        }
    }
}