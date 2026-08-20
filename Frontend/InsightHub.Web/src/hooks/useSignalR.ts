import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

export interface SignalRState {
    isConnected: boolean;
    notification: string | null;
    progress: { datasetId: string; percent: number; message: string } | null;
}

export function useSignalR(hubUrl: string = "http://localhost:5099/hubs/analysis"): SignalRState {
    const [isConnected, setIsConnected] = useState<boolean>(false);
    const [notification, setNotification] = useState<string | null>(null);
    const [progress, setProgress] = useState<{ datasetId: string; percent: number; message: string } | null>(null);

    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl)
            .withAutomaticReconnect()
            .build();

        connection
            .start()
            .then(() => {
                setIsConnected(true);
            })
            .catch(() => {
                setIsConnected(false);
            });

        connection.on("ReceiveAnalysisNotification", (msg: string) => {
            setNotification(msg);
        });

        connection.on("ReceiveDatasetProgress", (datasetId: string, percent: number, message: string) => {
            setProgress({ datasetId, percent, message });
        });

        return () => {
            connection.stop();
        };
    }, [hubUrl]);

    return { isConnected, notification, progress };
}
