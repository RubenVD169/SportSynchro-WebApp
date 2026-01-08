import { useParams, useNavigate } from "react-router-dom";
import { useLayoutEffect, useRef, useState } from "react";
import MatchCard from "../../components/matches/MatchCard";
import LiveMatchCard from "../../components/matches/LiveMatchCard";
import useLeagueSchedule from "../../hooks/useLeagueSchedule";
import { deriveMatchStatus, downloadSchedulePdf } from "../../services/matchService";
import SimpleBar from "simplebar-react";
import type SimpleBarCore from "simplebar-core";
import useLiveMatches from "../../hooks/useLiveMatches";
import {useLocation} from "react-router-dom";
import { IoArrowBack } from "react-icons/io5";

export default function LeagueDetailsPage() {
    const { leagueId, sportId } = useParams<{ leagueId: string; sportId: string }>();
    const location = useLocation();
    const navigate = useNavigate();
    const leagueName = location.state?.leagueName;

    const [activeTab, setActiveTab] =
        useState<"schedule" | "live">("schedule");

    const { matches, loading } =
        useLeagueSchedule(Number(leagueId));
    const { liveMatches, loading: liveLoading, }
        = useLiveMatches(Number(leagueId));

    const simpleBarRef = useRef<SimpleBarCore | null>(null);
    const liveSectionRef = useRef<HTMLElement | null>(null);
    const scheduledSectionRef = useRef<HTMLElement | null>(null);
    const hasAutoScrolledRef = useRef(false);

    const finishedMatches = matches.filter(
        (m) => deriveMatchStatus(m.status, m.matchDate) === "finished"
    );

    const scheduleLiveMatches = matches.filter(
        (m) => deriveMatchStatus(m.status, m.matchDate) === "live"
    );

    const scheduledMatches = matches.filter(
        (m) => deriveMatchStatus(m.status, m.matchDate) === "not-started"
    );

    function scrollToSection(sectionRef: React.RefObject<HTMLElement | null>) {
        if (
            !simpleBarRef.current ||
            !sectionRef.current
        ) {
            return;
        }

        const container =
            simpleBarRef.current.getScrollElement();

        if (!container) {
            return;
        }

        const cRect = container.getBoundingClientRect();
        const tRect = sectionRef.current.getBoundingClientRect();

        const targetTop =
            (tRect.top - cRect.top) + container.scrollTop;

        container.scrollTo({
            top: Math.max(0, targetTop),
            behavior: "auto",
        });
    }

    useLayoutEffect(() => {
        if (
            hasAutoScrolledRef.current ||
            loading ||
            activeTab !== "schedule" ||
            !simpleBarRef.current
        ) {
            return;
        }
        
        // Wait for refs to be set
        const timeoutId = setTimeout(() => {
            if (scheduleLiveMatches.length > 0 && liveSectionRef.current) {
                scrollToSection(liveSectionRef);
                hasAutoScrolledRef.current = true;
            } else if (scheduledMatches.length > 0 && scheduledSectionRef.current) {
                scrollToSection(scheduledSectionRef);
                hasAutoScrolledRef.current = true;
            }
        }, 0);
        
        return () => clearTimeout(timeoutId);
    }, [loading, activeTab, scheduleLiveMatches.length, scheduledMatches.length]);

    return (
        <div className="p-6 max-w-5xl mx-auto">
            <div className="mb-6">
                <button
                    onClick={() => navigate(`/sports/${sportId}`)}
                    className="mb-4 flex items-center gap-2 text-gray-400 hover:text-white transition hover:cursor-pointer"
                >
                    <IoArrowBack className="text-lg" />
                    <span className="text-sm">Back to sport</span>
                </button>
                <h1 className="text-2xl font-semibold text-white">
                    {leagueName}
                </h1>
                <p className="text-sm text-gray-400">
                    Matches & live results
                </p>

                <button
                    onClick={() => downloadSchedulePdf(Number(leagueId))}
                    className="px-4 py-2 text-sm rounded-md
                        bg-gray-800 text-gray-200
                        hover:bg-indigo-600 hover:text-white
                        transition hover:cursor-pointer">
                    Download schedule
                </button>
            </div>

            <div className="flex gap-2 mb-6">
                <button
                    onClick={() => {
                        setActiveTab("schedule");
                        requestAnimationFrame(() => {
                            if (scheduleLiveMatches.length > 0) {
                                scrollToSection(liveSectionRef);
                            } else if (scheduledMatches.length > 0) {
                                scrollToSection(scheduledSectionRef);
                            }
                        });
                    }}
                    className={`px-4 py-2 rounded text-sm font-medium
            ${activeTab === "schedule"
                            ? "bg-indigo-600 text-white"
                            : "bg-gray-800 text-gray-400 hover:text-white cursor-pointer"
                        }`}
                >
                    Schedule
                </button>

                <button
                    onClick={() => setActiveTab("live")}
                    className={`px-4 py-2 rounded text-sm font-medium 
                            ${activeTab === "live"
                            ? "bg-indigo-600 text-white"
                            : "bg-gray-800 text-gray-400 hover:text-white cursor-pointer"
                        }`}
                >
                    Live matches
                </button>
            </div>

            {activeTab === "schedule" && (
                <SimpleBar
                    ref={simpleBarRef}
                    style={{ maxHeight: "70vh" }}
                    className="space-y-6"
                >
                    {loading ? (
                        <p className="text-sm text-gray-500">
                            Loading matches…
                        </p>
                    ) : matches.length === 0 ? (
                        <p className="text-sm text-gray-500">
                            No schedule available.
                        </p>
                    ) : (
                        <>
                            {/* Finished */}
                            {finishedMatches.length > 0 && (
                                <section>
                                    <h3 className="text-sm text-gray-400 mb-2">
                                        Finished
                                    </h3>
                                    <div className="space-y-3">
                                        {finishedMatches.map((m) => (
                                            <MatchCard key={m.id} match={m} />
                                        ))}
                                    </div>
                                </section>
                            )}

                            {/* Live */}
                            {scheduleLiveMatches.length > 0 && (
                                <section ref={liveSectionRef}>
                                    <h3 className="text-sm font-semibold text-red-400 mb-2">
                                        Live
                                    </h3>
                                    <div className="space-y-3">
                                        {scheduleLiveMatches.map((m) => (
                                            <MatchCard key={m.id} match={m} />
                                        ))}
                                    </div>
                                </section>
                            )}

                            {/* Scheduled */}
                            {scheduledMatches.length > 0 && (
                                <section ref={scheduledSectionRef}>
                                    <h3 className="text-sm text-indigo-400 mb-2">
                                        Scheduled
                                    </h3>
                                    <div className="space-y-3">
                                        {scheduledMatches.map((m) => (
                                            <MatchCard key={m.id} match={m} />
                                        ))}
                                    </div>
                                </section>
                            )}
                        </>
                    )}
                </SimpleBar>
            )}

            {activeTab === "live" && (
                <div className="space-y-3">
                    <h3 className="text-sm font-semibold text-red-400 mb-2">
                        Live now
                    </h3>
                    {liveLoading ? (
                        <div className="flex items-center justify-center h-32">
                            <p className="text-sm text-gray-500">
                                Loading live matches…
                            </p>
                        </div>
                    ) : liveMatches.length === 0 ? (
                        <div className="flex items-center justify-center h-32">
                            <p className="text-sm text-gray-500">
                                No live matches at the moment.
                            </p>
                        </div>
                    ) : (
                        liveMatches.map((m) => (
                            <LiveMatchCard key={m.id} match={m} />
                        ))
                    )}
                </div>
            )}
        </div>
    );
}

