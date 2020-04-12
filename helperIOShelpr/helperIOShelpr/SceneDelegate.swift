//
//  SceneDelegate.swift
//  helperIOShelpr
//
//  Created by Sam Horn on 4/4/20.
//  Copyright © 2020 Sam Horn. All rights reserved.
//

import UIKit

class SceneDelegate: UIResponder, UIWindowSceneDelegate, UISplitViewControllerDelegate {

    var window: UIWindow?

//print(String(format: "%s", window.rootViewController ?? <#default value#>))
    func scene(_ scene: UIScene, willConnectTo session: UISceneSession, options connectionOptions: UIScene.ConnectionOptions) {
        // Use this method to optionally configure and attach the UIWindow `window` to the provided UIWindowScene `scene`.
        // If using a storyboard, the `window` property will automatically be initialized and attached to the scene.
        // This delegate does not imply the connecting scene or session are new (see `application:configurationForConnectingSceneSession` instead).
        guard let window = window else { return }
        
        guard window.rootViewController != nil
        
        //let root = window.rootViewController
        //guard let kiddo = root?.children[0] else {
          //  print("BITCH")
            //return
        //}
        //let nav = window.rootViewController as! UINavigationController
        //let login = nav.viewControllers[0]
        //let split = nav.viewControllers[1]
        
        //guard let topNav = (kiddo as! UINavigationController).topViewController else {
          //  return
        //}
        //jawnson = window.rootViewController
        //var splitViewController: UISplitViewController? = nil
        
        //if nav.viewControllers.count > 1 {
          //  guard let _splitViewController = (nav.viewControllers[1] as? UISplitViewController)  else {
            //    return
            //}
            //splitViewController = _splitViewController
        //}
        //else {
          //  return
        //}
       // guard let splitViewController = topNav as? UISplitViewController else {
         //   print("SUCK MY BALLS")
            //      return }
        //guard let navigationController = splitViewController?.viewControllers.last as? UINavigationController else { return }
        //navigationController.topViewController?.navigationItem.leftBarButtonItem = splitViewController?.displayModeButtonItem
        //navigationController.topViewController?.navigationItem.leftItemsSupplementBackButton = true
        //splitViewController?.delegate = self
        /*
        guard let nav = (window.rootViewController as? UINavigationController) else { return }
        var splitViewController: UISplitViewController? = nil
        
        if nav.viewControllers.count > 1 {
            guard let _splitViewController = (nav.viewControllers[1].splitViewController)  else {
                return
            }
            splitViewController = _splitViewController
        }
        else {
            return
        }

        let masterNavigationController = splitViewController?.viewControllers[0] as! UINavigationController
        let controller = masterNavigationController.topViewController as! MasterViewController
        controller.managedObjectContext = (UIApplication.shared.delegate as? AppDelegate)?.persistentContainer.viewContext
        
        let detailNavigationController = splitViewController?.viewControllers[1] as! UINavigationController
        //let controllerboi =  (splitViewController?.viewControllers[1] as! UINavigationController).topViewController
        let detailController = detailNavigationController.topViewController as! DetailViewController
        detailController.navigationItem.leftBarButtonItem = splitViewController?.displayModeButtonItem
        detailController.navigationItem.leftItemsSupplementBackButton = true
        detailController.managedObjectContext = (UIApplication.shared.delegate as? AppDelegate)?.persistentContainer.viewContext
 */
 
    }

    func sceneDidDisconnect(_ scene: UIScene) {
        // Called as the scene is being released by the system.
        // This occurs shortly after the scene enters the background, or when its session is discarded.
        // Release any resources associated with this scene that can be re-created the next time the scene connects.
        // The scene may re-connect later, as its session was not neccessarily discarded (see `application:didDiscardSceneSessions` instead).
    }

    func sceneDidBecomeActive(_ scene: UIScene) {
        // Called when the scene has moved from an inactive state to an active state.
        // Use this method to restart any tasks that were paused (or not yet started) when the scene was inactive.
    }

    func sceneWillResignActive(_ scene: UIScene) {
        // Called when the scene will move from an active state to an inactive state.
        // This may occur due to temporary interruptions (ex. an incoming phone call).
    }

    func sceneWillEnterForeground(_ scene: UIScene) {
        // Called as the scene transitions from the background to the foreground.
        // Use this method to undo the changes made on entering the background.
    }

    func sceneDidEnterBackground(_ scene: UIScene) {
        // Called as the scene transitions from the foreground to the background.
        // Use this method to save data, release shared resources, and store enough scene-specific state information
        // to restore the scene back to its current state.

        // Save changes in the application's managed object context when the application transitions to the background.
        (UIApplication.shared.delegate as? AppDelegate)?.saveContext()
    }

    // MARK: - Split view

    func splitViewController(_ splitViewController: UISplitViewController, collapseSecondary secondaryViewController:UIViewController, onto primaryViewController:UIViewController) -> Bool {
        guard let secondaryAsNavController = secondaryViewController as? UINavigationController else { return false }
        guard let topAsDetailController = secondaryAsNavController.topViewController as? DetailViewController else { return false }
        if topAsDetailController.detailItem == nil {
             //Return true to indicate that we have handled the collapse by doing nothing; the secondary controller will be discarded.
             return true
        }
        return false
    }

}

