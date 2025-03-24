import EventForm from "@/components/shared/EventForm";
// import { auth, currentUser } from "@clerk/nextjs/server";
import { cookies } from 'next/headers'
import { redirect } from "next/navigation";
import React from "react";

const CreateEvent = async () => {
  // const { userId } = (await auth()) as { userId: string };
  const token = cookies().get('authToken')?.value
  const verifyResponse = await fetch(
    `${process.env.API_BASE_URL}/auth/verify`, 
    {
      headers: {
        Cookie: `authToken=${token}`
      }
    }
  )

  if (!verifyResponse.ok) {
    redirect('/login')
  }
  
  const user = await verifyResponse.json()
  
  if (user.role !== 'Host') {
    redirect('/events')
  }

  return (
    <>
      <section className="bg-primary-50 bg-dotted-pattern bg-cover bg-center py-5 md:py-1">
        <h3 className="wrapper h3-bold text-center sm:text-left">
          Create Event
        </h3>
      </section>
      <div className="wrapper my-8">
        <EventForm userId={user.id} type="Create" />
      </div>
    </>
  );
};

export default CreateEvent;
