import { NextResponse } from 'next/server'

export async function middleware(request) {
  const { pathname } = request.nextUrl
  
  // 保护创建活动路由
  if (pathname.startsWith('/create-event')) {
    const token = request.cookies.get('authToken')
    
    try {
      // 调用ASP.NET验证接口
      const verifyResponse = await fetch(
        `${process.env.API_BASE_URL}/auth/verify`, 
        {
          headers: {
            Cookie: `authToken=${token?.value}`
          }
        }
      )
      
      if (!verifyResponse.ok) {
        throw new Error('Invalid token')
      }
      
      const user = await verifyResponse.json()
      if (user.role !== 'Host') {
        return NextResponse.redirect(new URL('/events', request.url))
      }
    } catch (error) {
      return NextResponse.redirect(new URL('/login', request.url))
    }
  }
  
  return NextResponse.next()
}