"use server";
import { CreateEventParams } from "@/types";
import { handleError } from "../utils";

export const createEvent = async ({
  event,
  userId,
  path,
}: CreateEventParams) => {
    try {
        
    } catch (error) {
        handleError(error)
    }
};
