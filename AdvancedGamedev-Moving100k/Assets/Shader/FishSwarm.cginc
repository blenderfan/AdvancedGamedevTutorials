

#ifndef FISH_SWARM_INCLUDED
#define FISH_SWARM_INCLUDED

struct FishData {
    float velocity;
    float angularVelocity;
    float timeOffset;
    float3 offset;
    float scale;
    float4 color;
};


float4x4 InsertRotationMatrix(float4x4 m, float3x3 n) {
    
    float4x4 copy = m;
    
    copy._m00_m01_m02 = n._m00_m01_m02;
    copy._m10_m11_m12 = n._m10_m11_m12;
    copy._m20_m21_m22 = n._m20_m21_m22;
    
    return copy;
}


float3x3 CreateRotationMatrix(float3 axis, float angle) {
    
    float3x3 m;
    
    float s, c;
    sincos(angle, s, c);
   
    float cinv = 1 - c;
    float xycinv = axis.x * axis.y * cinv;
    float xzcinv = axis.x * axis.z * cinv;
    float yzcinv = axis.y * axis.z * cinv;
    
    m._m00 = c + axis.x * axis.x * cinv;
    m._m01 = xycinv - axis.z * s;
    m._m02 = xzcinv + axis.y * s;
    
    m._m10 = xycinv + axis.z * s;
    m._m11 = c + axis.y * axis.y * cinv;
    m._m12 = yzcinv - axis.x * s;
    
    m._m20 = xzcinv - axis.y * s;
    m._m21 = yzcinv + axis.x * s;
    m._m22 = c + axis.z * axis.z * cinv;
    
    return m;
}

float4x4 CreateRotationAxisAngle(float4x4 m, float3 axis, float angle) 
{
    float s, c;
    sincos(angle, s, c);
   
    float cinv = 1 - c;
    float xycinv = axis.x * axis.y * cinv;
    float xzcinv = axis.x * axis.z * cinv;
    float yzcinv = axis.y * axis.z * cinv;
    
    m._m00 = c + axis.x * axis.x * cinv;
    m._m01 = xycinv - axis.z * s;
    m._m02 = xzcinv + axis.y * s;
    
    m._m10 = xycinv + axis.z * s;
    m._m11 = c + axis.y * axis.y * cinv;
    m._m12 = yzcinv - axis.x * s;
    
    m._m20 = xzcinv - axis.y * s;
    m._m21 = yzcinv + axis.x * s;
    m._m22 = c + axis.z * axis.z * cinv;
    
    return m;
}

#endif